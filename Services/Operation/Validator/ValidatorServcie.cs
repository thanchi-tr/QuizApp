using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Services.Cache;
using QuizApp.Services.ConcreteStrategies.MultipleChoice;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO;
using QuizApp.Services.Operation.Provider;
using System.Text.Json;

namespace QuizApp.Services.Operation.Validator
{
    public class ValidatorServcie : IValidateService
    {
        private IInformationCache<AnswersDTO> _informationCache;
        private InformationProvider<AnswersDTO, Answer> _answerProvider;
        private IServiceProvider _serviceProvider;

        public ValidatorServcie(IInformationCache<AnswersDTO> informationCache
                            , InformationProvider<AnswersDTO, Answer> answerProvider
                            , IServiceProvider serviceProvider)
        {
            _informationCache = informationCache;
            _answerProvider = answerProvider;
            _serviceProvider = serviceProvider;
        }



        /// <summary>
        /// </summary>
        /// <param name="receiveAnswer"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ResponseValidatePayload Validate(string serializedReceivedAnswer, string collectionId, string questionId)
        {
            var requestPayload = JsonSerializer
                            .Deserialize<SerializedAttemptDTO>(serializedReceivedAnswer);
            // Check for missing detail
            if (requestPayload == null
                || !requestPayload.IsValid()) return ResponseValidatePayload.Default;

            // get the data from cache
            var answer = _informationCache.Get(collectionId);
            if (answer == null)
            {
                answer = _answerProvider.Get(collectionId);
                _informationCache.Cache(answer, collectionId);
            }
            // find the answer in the cache answer.
            foreach (var serializedAnswer in answer.Answer)
            {
                // obtain the actual structure of the answer acording to its type
                // aka multiple choice
                var correctAnswer = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(serializedAnswer);
                var attempt = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(requestPayload.Answer);

                if (correctAnswer != null && attempt != null && // match their id
                    correctAnswer.QuestionId
                        .Equals(attempt.QuestionId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    /// Inject the strategy and use that to validate the result
                    /// BRUTEFORCE + NAIVE :: compare answer every request
                    var strategy = _serviceProvider.GetService(typeof(ValidateMultipleChoiceStrategy)) as IValidatingStrategy<MultipleChoiceAnswerDTO>;
                    if (strategy != null)
                    {
                        return strategy.Validate(correctAnswer, requestPayload.Answer);
                    }
                    else
                    {
                        throw new InvalidOperationException("Validation strategy not found for this answer type.");
                    }
                }
            }
            return ResponseValidatePayload.Default;

        }
    }
}
