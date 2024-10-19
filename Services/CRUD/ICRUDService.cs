namespace QuizApp.Services.CRUD
{
    public interface ICRUDService<T>
    {
        public List<T> GetAll();
        public bool CreateQuestion(string questionStr, string serializedCollectionId);
    }
}
