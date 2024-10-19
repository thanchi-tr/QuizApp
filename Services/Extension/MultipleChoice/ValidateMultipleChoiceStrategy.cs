using QuizApp.Model.DTO.External;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO;
using QuizApp.Services.Operation.Validator;
using System.Text.Json;

namespace QuizApp.Services.ConcreteStrategies.MultipleChoice
{
    public class ValidateMultipleChoiceStrategy : IValidatingStrategy<MultipleChoiceAnswerDTO>
    {
        /// <summary>
        ///  This simple logic can be move to the front end server side
        ///  @todo: compute the logic as the whole and send it along side the question since
        ///  it does not have any performace improve to keep it 
        /// </summary>
        /// <param name="valiadateAnswer"></param>
        /// <param name="receiveAnswer"> still in the form of MutipleChoiceValidationAnswer <- but serialized into a string</param>
        /// <returns></returns>
        public ResponseValidatePayload Validate(MultipleChoiceAnswerDTO valiadateAnswer, string receiveAnswer)
        {
            if (receiveAnswer == null)
                return ResponseValidatePayload.Default;
            var attempt = JsonSerializer.Deserialize<MultipleChoiceAnswerDTO>(receiveAnswer);
 
            return (attempt == null)
                ? ResponseValidatePayload.Default
                : new ResponseValidatePayload
                    {
                        QuesitonId = valiadateAnswer.QuestionId,
                        result = valiadateAnswer.Answer.Equals(attempt.Answer),
                        Correct = valiadateAnswer.Answer
                    };

        }
    }
}
