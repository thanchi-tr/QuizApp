namespace QuizApp.Services.CRUD
{
    public interface ICRUDService<T>
    {
        public List<T> GetAll();
    }
}
