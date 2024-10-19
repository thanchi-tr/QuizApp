using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
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
        /// receive answer are in the form of {format: questionFormat, answer: "serialized MultipleChoiceValidateAnswer"}
        /// </summary>
        /// <param name="receiveAnswer"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ValidateResultDTO Validate(string receiveAnswer)
        {
            var answer = JsonSerializer.Deserialize<RestfulAnswer>(receiveAnswer);
            var result = new ValidateResultDTO
            {
                QuesitonId = "",
                Correct ="",
                result = false
            };
            // mising Key parameter for operation
            if (string.IsNullOrEmpty(answer.Answer) || string.IsNullOrEmpty(answer.CollectionId) || string.IsNullOrEmpty(answer.Type))
            {
                return result;
            }
            //get the correct answer
            AnswersDTO answerDTO;
            var validatedAnswers = _informationCache.Get(answer.CollectionId);
            if (validatedAnswers == null)
            {
                validatedAnswers = _answerProvider.Get(answer.CollectionId);
                _informationCache.Cache(validatedAnswers, answer.CollectionId);
            }
            // find the answer in the cache answer.
            foreach (var serializedAns in validatedAnswers.Answer)
            {
                var validatedAnswer = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(serializedAns);

                var attemptAnswer = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(answer.Answer);
                if (validatedAnswer != null && attemptAnswer != null &&
                    new Guid(validatedAnswer.QuestionId).Equals(new Guid(attemptAnswer.QuestionId)))
                {
                    var strategy = _serviceProvider.GetService(typeof(ValidateMultipleChoiceStrategy)) as IValidatingStrategy<MultipleChoiceAnswerDTO>;

                    if (strategy != null)
                    {
                        return strategy.Validate(validatedAnswer, answer.Answer);
                    }
                    else
                    {
                        throw new InvalidOperationException("Validation strategy not found for this answer type.");
                    }
                }
            }
            ///@todo: implement the strategy pattern later when we add another question format
            return result;

        }
    }
}
