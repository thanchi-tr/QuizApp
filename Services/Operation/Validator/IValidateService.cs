using QuizApp.Model.DTO.External;

namespace QuizApp.Services.Operation.Validator
{
    public interface IValidateService
    {
        /// <summary>
        /// Answer recieve are in form of serialized json
        /// </summary>
        /// <param name="receiveAnswer"> String</param>
        /// <returns></returns>
        public ResponseValidatePayload Validate(string receiveAnswer);
    }
}
