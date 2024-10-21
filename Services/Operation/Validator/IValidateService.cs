using QuizApp.Model.DTO.External;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Model.DTO.Internal;

namespace QuizApp.Services.Operation.Validator
{
    public interface IValidateService
    {
        /// <summary>
        /// Answer recieve are in form of serialized json
        /// </summary>
        /// <param name="receiveAnswer"> String</param>
        /// <returns></returns>
        public BusinessToPresentationLayerDTO<ResponseValidatePayload> Validate(SerializedAttemptDTO abstractAttempt, string collectionId, string questionId);



    }
}
