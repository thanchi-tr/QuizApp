using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.DTO;
using QuizApp.Services.ConcreteStrategies.MultipleChoice;

namespace QuizApp.Services.Operation.Provider
{
    public class AnswerProvider : InformationProvider<AnswersDTO, Model.Domain.Answer>
    {
        public AnswerProvider(IdeaSpaceDBContext context) : base(context)
        {
        }
        // read data from the context and store it there
        private Model.Domain.Collection? _collection;
        private List<Model.Domain.Answer?> _answer = new List<Model.Domain.Answer?>();

        /// <summary>
        /// From the question ID get the correct answer (or a packet substantial to 
        /// compute a correct answer)
        /// </summary>
        /// <param name="id"></param>
        protected override async void GetRawData(string id)
        {
            _collection = await _context.Collections
                .FirstOrDefaultAsync(q => q.CollectionId == new Guid(id));

            // obtain all the question
            if (_collection != null)
            {
                foreach (var question in await _context.Questions
                    .AsNoTracking()
                    .Where(q => q.CollectionId == _collection.CollectionId)
                    .Include(q => q.Answer) // eager loading
                    .ToListAsync())
                {
                    if (question != null && question.Answer != null)
                    {
                        _answer.Add(question.Answer);
                    }
                }

            }
        }

        protected override void Process()
        {
            _information = new AnswersDTO
            {
                Title = _collection.CollectionId.ToString(),

            };
            foreach (var answer in _answer)
            {

                ///@todo: find a way  to solve the issue with routing the different question type strategy
                /// this implementation is high specific to multiple choice.
                /// will receive a string with queston
                if (answer != null)
                {
                    var ConcreteAnswer = ExtractFromRawData(new ExtractMultipleChoiceAnswerStrategy(), answer);
                    _information.Answer.Add(ConcreteAnswer);
                }
            }
        }
    }
}
