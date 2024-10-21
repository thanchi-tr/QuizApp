using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.Domain;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuizApp.Services.CRUD
{
    public class CollectionService : ICRUDService<CollectionDTO>
    {
        private readonly IdeaSpaceDBContext _context;
        private readonly IMapper _mapper;

        public CollectionService(IdeaSpaceDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // Map integer to Domain.Level enum
        private Level MapIntToLevel(int level)
        {
            if (Enum.IsDefined(typeof(Level), level))
            {
                return (Level)level;
            }

            throw new ArgumentException($"Invalid level: {level}");
        }

        

        /// <summary>
        /// get the list off all the DTO in the db
        /// </summary>
        /// <returns></returns>
        public async Task<List<CollectionDTO>> GetAllCollectionAsync()
        {
            var rawList = await _context.Collections.ToListAsync();
            var collectionDTOs = rawList
                    .Where(raw => raw != null)
                    .Select(raw => _mapper.Map<CollectionDTO>(raw))
                    .ToList();
            return collectionDTOs;
        }

        /// <summary>
        /// Parse the question Serialized JSON into questionCreate DTO
        /// 
        /// questionStr currently follow the MC CreateQuestionDTO request
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task<bool> CreateQuestion(string questionStr, string serializedCollectionId)
        {
            if (string.IsNullOrEmpty(questionStr))
                return false;
            // parse the serialized question into question create dto
            try{
                MultipleChoiceQuestionAnswerDTO question = JsonSerializer.Deserialize<MultipleChoiceQuestionAnswerDTO>(questionStr);
                var newQuestion = new Question
                {
                    CollectionId = new Guid(serializedCollectionId),
                    Value = question.Question,
                    Level = MapIntToLevel(0),
                    LastRevision = DateOnly.FromDateTime(DateTime.Now)
                };
                await _context.Questions.AddAsync(newQuestion);

                // add the corresponded answer
                await _context.Answers.AddAsync(
                    new Answer
                    {
                        QuestionId = newQuestion.QuestionId,
                        Value = JsonSerializer.Serialize(
                            new DatabaseMultiplechoiceAnswer
                            {
                                Correct= question.Correct,
                                Incorrect= question.Incorrect
                            }
                            ),
                        Type = "MultipleChoice",
                    }
                );
                _context.SaveChanges();
                return true;
            }
            catch (JsonException e)
            {
                Console.WriteLine("JSON Deserialization Error: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected error occurred: " + e.Message);
            }
            return false;
        }

        /// <summary>
        /// Create the new collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> CreateCollection(string name)
        {
            try
            {
                await _context.AddAsync(new Collection
                {
                    Name = name,
                    //As of now, this is a dummy value.
                    // My intention is for this to be the creator of the quiz. who has the right to
                    // edit the quiz
                    UserId = new Guid("2A8C2FD1-4443-4E0E-AC39-062F7C1C75D3")
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteQuestion(string collectionId, string questionId)
        {
            if (string.IsNullOrEmpty(collectionId) || string.IsNullOrEmpty(questionId)) {
                return false;
            }
            // find the question we want to delete (eager loading with its answer)
            var target = await _context.Questions
                .Include(q => q.Answer)
                .FirstOrDefaultAsync(q => q.QuestionId == new Guid(questionId));
            if (target == null) {
                return false;
            }
            _context.Answers.Remove(target.Answer); // Delete all answers
            _context.Questions.Remove(target);            // Delete the question
            await _context.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Handle the Edit Question
        ///     The Priority of Error( dictate the order of when the method stop)
        ///         (1) missing questionDetail/ questionId
        ///         (2) questionDetail is of correct format
        ///         (3) the Question does not exist
        ///         (4) no other problem occur
        /// </summary>
        /// <param name="questionStr">Serialize Json object/ DTO typeof(MultipleChoiceQuestionAnswerDTO):: Locate in Services.Extension</param>
        /// <param name="questionId">string value of Guid:: key of an entry in Question table</param>
        /// <returns>
        ///     ["Not found"] : the target resource (actual question entry) does not exist
        ///     ["questionDetail" and/or "questionId" and/or "Format"] : invalid request, either missing the prop or wrong format
        ///     ["Other"] : other error that need to be investigate @20/10/24
        /// </returns>
        public async Task<string[]> EditQuestion(string serializedMCQuestionAnswerDTO, string questionId)
        {
            var missingProp = new List<string>();
            if (string.IsNullOrEmpty(serializedMCQuestionAnswerDTO)) missingProp.Add("questionDetail");
            if (string.IsNullOrEmpty(questionId)) missingProp.Add("QuestionId");
            if (missingProp.Count > 0)
            {
                return [.. missingProp]; // will result in a 400 error code
            }
            
            try
            {
                MultipleChoiceQuestionAnswerDTO MCQuestionAnswerDTO = JsonSerializer.Deserialize<MultipleChoiceQuestionAnswerDTO>(serializedMCQuestionAnswerDTO);
                if (MCQuestionAnswerDTO == null)
                    return ["FormatError", "Null serializedMCQuestionAnswerDTO"];

                // find entity in database
                var target = await _context.Questions
                        .Include(q => q.Answer) //eager loading save 1 round query
                        .FirstOrDefaultAsync(q => q.QuestionId.Equals(new Guid(questionId)));

                if (target == null)
                    return ["Not found"];

                
                target.Value = MCQuestionAnswerDTO.Question;
                target.Answer.Value = JsonSerializer.Serialize( // when introduce more type, need to abstract this
                            new DatabaseMultiplechoiceAnswer
                            {
                                Correct = MCQuestionAnswerDTO.Correct,
                                Incorrect = MCQuestionAnswerDTO.Incorrect
                            }
                    );
                 await _context.SaveChangesAsync();
                
            }
            catch (JsonException DetailError)
            {
                return ["FormatError", DetailError.ToString()];
            }
            catch (Exception e)
            {
                return ["Other", e.ToString()]; // result in sending an un-know error notif back 
            }
            return []; // successful mod
        }
    }
}
