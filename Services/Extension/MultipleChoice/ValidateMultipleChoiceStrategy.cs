using QuizApp.Model.DTO;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO;
using QuizApp.Services.Operation.Validator;
using System.Text.Json;

namespace QuizApp.Services.ConcreteStrategies.MultipleChoice
{
    public class ValidateMultipleChoiceStrategy : IValidatingStrategy<MultipleChoiceAnswerDTO>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valiadateAnswer"></param>
        /// <param name="receiveAnswer"> still in the form of MutipleChoiceValidationAnswer <- but serialized into a string</param>
        /// <returns></returns>
        public ValidateResultDTO Validate(MultipleChoiceAnswerDTO valiadateAnswer, string receiveAnswer)
        {


            if (receiveAnswer == null)
                return default;

            var attempt = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(receiveAnswer);
            if (attempt == null)
                return default;

            return new ValidateResultDTO
            {
                QuesitonId = valiadateAnswer.QuestionId,
                result = valiadateAnswer.Answer.Equals(attempt.Answer),
                Correct = valiadateAnswer.Answer
            };

        }
    }
}
