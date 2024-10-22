using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Model.DTO.Internal;

namespace QuizApp.Services.CRUD
{
    public interface ICRUDService<T>
    {
        public Task<BusinessToPresentationLayerDTO<CollectionDTO[]>> GetAllCollectionAsync(string requestorId);
        public Task<BusinessToPresentationLayerDTO<string>> CreateQuestion(QuestionAnswerDTO questionWithAnswer, string serializedCollectionId, string requestorId);
        public Task<BusinessToPresentationLayerDTO<string>> CreateCollection(string name, string requestorId);

        public Task<BusinessToPresentationLayerDTO<string>> DeleteQuestion(string questionId, string requestorId);
        public Task<BusinessToPresentationLayerDTO<string[]>> EditQuestion(string serializedMCQuestionAnswerDTO, string questionId);


    }
}
