using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Model.DTO.Internal;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.Domain;
using QuizApp.Services.Extension.MultipleChoice.Model.DTO;
using System.Text.Json;

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
        public async Task<BusinessToPresentationLayerDTO<CollectionDTO[]>> GetAllCollectionAsync(string requestorId)
        {
            
            var rawList = await _context.Collections
                    .AsNoTracking()
                    .Where(c => c.UserId == new Guid(requestorId))
                    .ToListAsync();

            var collectionDTOs = rawList
                    .Where(raw => raw != null)
                    .Select(raw => _mapper.Map<CollectionDTO>(raw))
                    .ToList()
                    .ToArray();
            return new BusinessToPresentationLayerDTO<CollectionDTO[]>(true, "", collectionDTOs);
        }

        /// <summary>
        /// Parse the question Serialized JSON into questionCreate DTO
        /// 
        /// questionStr currently follow the MC CreateQuestionDTO request
        /// </summary>
        /// <param name="questionWithAnswer"></param>
        /// <param name="serializedCollectionId">string version of guid</param>
        /// <param name="RequestorId">the person who make the request</param>
        /// <returns></returns>
        public async Task<BusinessToPresentationLayerDTO<string>> CreateQuestion(QuestionAnswerDTO questionWithAnswer, string serializedCollectionId, string RequestorId)
        {
            if (questionWithAnswer == null ||
                    string.IsNullOrEmpty(questionWithAnswer.Question) ||
                    string.IsNullOrEmpty(questionWithAnswer.Type) || // Todo: implement a check if the type receive is valid
                    string.IsNullOrEmpty(questionWithAnswer.SerializedAnswer)) 
                return new BusinessToPresentationLayerDTO<string>(false, "", "");
            // parse the serialized question into question create dto
            try{
                /* we can use a apply a strat selection here to parse the question*/
#pragma warning disable CS8600 // already ensure questionWithAnswer.SerializedAnswer not null
                MultipleChoiceCreatedAnswerDTO answer = JsonSerializer
                                                            .Deserialize<MultipleChoiceCreatedAnswerDTO>(questionWithAnswer.SerializedAnswer);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                // check if the collection exist
                var target = await _context.Collections
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CollectionId == new Guid(serializedCollectionId));
                if(target == null)
                {
                    return new BusinessToPresentationLayerDTO<string>(false, "Not found", "");
                }
                if(!target.UserId.ToString().Contains(RequestorId)) // user is not owner of collection, therefore have no right to edit it
                {
                    return new BusinessToPresentationLayerDTO<string>(false, "Un-authorise", "");
                }
                    /*Todo move this into repository*/
                    var newQuestion = new Question
                {
                    CollectionId = new Guid(serializedCollectionId),
                    Value = questionWithAnswer.Question,
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
                                Correct= answer.Correct,
                                Incorrect= answer.Incorrect
                            }
                            ),
                        Type = "MultipleChoice",
                    }
                );
                _context.SaveChanges();
                }
            catch (JsonException e)
            {
                return new BusinessToPresentationLayerDTO<string>(false, "Format", e.ToString());
            }
            catch (Exception e)
            {
                return new BusinessToPresentationLayerDTO<string>(false, "Other", e.ToString());
            }
            return new BusinessToPresentationLayerDTO<string>(true, "", "");

        }

        /// <summary>
        /// Create the new collection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<BusinessToPresentationLayerDTO<string>> CreateCollection(string name, string requestorId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new BusinessToPresentationLayerDTO<string>(false, "Missing Prop", "");
            }
            try
            {
                await _context.AddAsync(new Collection
                {
                    Name = name,
                    //As of now, this is a dummy value.
                    // My intention is for this to be the creator of the quiz. who has the right to
                    // edit the quiz
                    UserId = new Guid(requestorId) // requestor is authenticated (they exist in our db)
                });
                await _context.SaveChangesAsync();
                return new BusinessToPresentationLayerDTO<string>(true, "", "");

            }
            catch (Exception ex) 
            {
                return new BusinessToPresentationLayerDTO<string>(false, "Other", ex.ToString());
            }
        }

        /// <summary>
        /// Delete the question (if exist) and its corresponding answer
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public async Task<BusinessToPresentationLayerDTO<string>> DeleteQuestion(string questionId, string requestorId)
        {
            
            if (string.IsNullOrEmpty(questionId))
                return new BusinessToPresentationLayerDTO<string>(false, "Missing Prop", "questionId");


            // find the question we want to delete (eager loading with its answer)
            var target = await _context.Questions
                .AsNoTracking()
                .Include(q => q.Collection)
                .Include(q => q.Answer)
                .FirstOrDefaultAsync(q => q.QuestionId == new Guid(questionId));
            
            if (target == null) {
                return new BusinessToPresentationLayerDTO<string>(false, "Not found", "");
            }
            if (target.Collection.UserId.ToString().Contains(requestorId))
            {
                return new BusinessToPresentationLayerDTO<string>(false, "Un-authorize", "");
            }
            _context.Answers.Remove(target.Answer); // Delete all answers
            _context.Questions.Remove(target);      // Delete the question
            await _context.SaveChangesAsync();

            return new BusinessToPresentationLayerDTO<string>(true, "", "");
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
        public async Task<BusinessToPresentationLayerDTO<string[]>> EditQuestion(string serializedMCQuestionAnswerDTO, string questionId)
        {
            var missingProp = new List<string>();
            if (string.IsNullOrEmpty(serializedMCQuestionAnswerDTO)) missingProp.Add("questionDetail");
            if (string.IsNullOrEmpty(questionId)) missingProp.Add("QuestionId");
            if (missingProp.Count > 0)
            {
                return new BusinessToPresentationLayerDTO<string[]>(false, "Missing neccesary Properties", missingProp.ToArray());

            }

            try
            {
#pragma warning disable CS8600 // We already check if the serializedMCQuestionAnswerDTO is not null and return.
                MultipleChoiceQuestionAnswerDTO MCQuestionAnswerDTO = JsonSerializer.Deserialize<MultipleChoiceQuestionAnswerDTO>(serializedMCQuestionAnswerDTO);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (MCQuestionAnswerDTO == null)
                    return new BusinessToPresentationLayerDTO<string[]>(false, "Missing neccesary Properties", ["Format"]);

                // find entity in database
                // @todo: moving these into the repository
                var target = await _context.Questions
                        .Include(q => q.Answer) //eager loading save 1 round query
                        .FirstOrDefaultAsync(q => q.QuestionId.Equals(new Guid(questionId)));

                if (target == null)
                    return new BusinessToPresentationLayerDTO<string[]>(false, "Not found", []);


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
                return new BusinessToPresentationLayerDTO<string[]>(false, "Missing neccesary Properties", ["Format", DetailError.ToString()]);
            }
            catch (Exception e)
            {
                return new BusinessToPresentationLayerDTO<string[]>(false, "Other", [e.ToString()]);

            }
            return new BusinessToPresentationLayerDTO<string[]>(true, "", []);

        }
    }
}
