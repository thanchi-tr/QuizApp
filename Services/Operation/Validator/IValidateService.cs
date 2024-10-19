using QuizApp.Model.DTO;

namespace QuizApp.Services.Operation.Validator
{
    public interface IValidateService
    {
        /// <summary>
        /// Answer recieve are in form of serialized json
        /// </summary>
        /// <param name="receiveAnswer"> String</param>
        /// <returns></returns>
        public ValidateResultDTO Validate(string receiveAnswer);
    }
}
