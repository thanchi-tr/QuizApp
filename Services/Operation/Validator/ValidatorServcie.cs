using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Model.DTO.Internal;
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
        /// Validate the abstract answer using its type.
        /// @todo: implement the asynchronous behaviour
        /// </summary>
        /// <param name="abstractAttempt"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BusinessToPresentationLayerDTO<ResponseValidatePayload> Validate(SerializedAttemptDTO abstractAttempt, string collectionId, string questionId)
        {
            var missingProp = new List<string>();
            if (string.IsNullOrEmpty(collectionId)) missingProp.Add("collectionId");
            if (string.IsNullOrEmpty(questionId)) missingProp.Add("questionId");
            if (missingProp.Count > 0)
            {
                return new BusinessToPresentationLayerDTO<ResponseValidatePayload>(false, "Missing neccesary Properties", ResponseValidatePayload.Default);

            }

            // Check for missing detail
            if (!abstractAttempt.IsValid()) 
                return new BusinessToPresentationLayerDTO<ResponseValidatePayload>(false, "", ResponseValidatePayload.Default);

            

            // get the data from cache
            var answer = _informationCache.Get(collectionId);
            if (answer == null)
            {
                answer = _answerProvider.Get(collectionId);
                if(answer == null)
                    return new BusinessToPresentationLayerDTO<ResponseValidatePayload>(false, "Not found", ResponseValidatePayload.Default);
                _informationCache.Cache(answer, collectionId);
            }
            // find the answer in the cache answer.
            foreach (var serializedAnswer in answer.Answer)
            {
                // obtain the actual structure of the answer acording to its type
                // aka multiple choice
                var correctAnswer = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(serializedAnswer);
                var attempt = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(abstractAttempt.Answer);

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
                        return new BusinessToPresentationLayerDTO<ResponseValidatePayload>(true, "", strategy.Validate(correctAnswer, abstractAttempt.Answer));
                    }
                    else
                    {
                        throw new InvalidOperationException("Validation strategy not found for this answer type.");
                    }
                }
            }
            return new BusinessToPresentationLayerDTO<ResponseValidatePayload>(false, "Other", ResponseValidatePayload.Default);

        }
    }
}
