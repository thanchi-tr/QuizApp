using AutoMapper;
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
        public List<CollectionDTO> GetAll()
        {
            var rawList = _context.Collections.ToList();
            List<CollectionDTO> collectionDTOs = new List<CollectionDTO>();
            foreach (var raw in rawList)
            {
                if(raw != null) 
                    collectionDTOs.Add(_mapper.Map<CollectionDTO>(raw));
            }
            return collectionDTOs;
        }

        /// <summary>
        /// Parse the question Serialized JSON into questionCreate DTO
        /// 
        /// questionStr currently follow the MC CreateQuestionDTO request
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public bool CreateQuestion(string questionStr, string serializedCollectionId)
        {
            if (string.IsNullOrEmpty(questionStr))
                return false;
            // parse the serialized question into question create dto
            try{
                MChoiceQuestionCreateDTO question = JsonSerializer.Deserialize<MChoiceQuestionCreateDTO>(questionStr);
                var newQuestion = new Question
                {
                    CollectionId = new Guid(serializedCollectionId),
                    Value = question.Question,
                    Level = MapIntToLevel(0),
                    LastRevision = DateOnly.FromDateTime(DateTime.Now)
                };
                _context.Questions.Add(newQuestion);
                 _context.SaveChanges();

                // add the corresponded answer
                _context.Answers.Add(
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

        public bool CreateCollection(string name)
        {
            return true;
        }
    }
}
