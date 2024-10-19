using QuizApp.Model.DTO;

namespace QuizApp.Services.Cache
{
    public interface IInformationCache<T>
    {
        public T Get(string id);
        public void Cache(T answer, string id);
    }
}
