using QuizApp.Model.DTO.External;

namespace QuizApp.Services.Operation.Validator
{
    public interface IValidatingStrategy<T>
    {
        /// <summary>
        /// Check the receive answer (forward from the controller in form serialized json
        /// Check it against correct answer
        /// </summary>
        /// <param name="valiadateAnswer"></param>
        /// <param name="receiveAnswer"></param>
        /// <returns></returns>
        public ResponseValidatePayload Validate(T valiadateAnswer, string receiveAnswer);
    }
}
