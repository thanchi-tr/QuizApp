using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Model.DTO.Internal;

namespace QuizApp.Services.CRUD
{
    public interface ICRUDService<T>
    {
        public Task<BusinessToPresentationLayerDTO<CollectionDTO[]>> GetAllCollectionAsync();
        public Task<BusinessToPresentationLayerDTO<string>> CreateQuestion(QuestionAnswerDTO questionWithAnswer, string serializedCollectionId);
        public Task<BusinessToPresentationLayerDTO<string>> CreateCollection(string name);

        public Task<BusinessToPresentationLayerDTO<string>> DeleteQuestion(string questionId);
        public Task<BusinessToPresentationLayerDTO<string[]>> EditQuestion(string serializedMCQuestionAnswerDTO, string questionId);


    }
}
