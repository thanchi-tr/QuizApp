using QuizApp.Model.DTO;

namespace QuizApp.Services.CRUD
{
    public interface ICRUDService<T>
    {
        public Task<List<T>> GetAllCollectionAsync();
        public Task<bool> CreateQuestion(string questionStr, string serializedCollectionId);
        public Task<bool> CreateCollection(string name);

        public Task<bool> DeleteQuestion(string collectionId, string questionId);
        public Task<string[]> EditQuestion(string questionStr, string questionId);


    }
}
