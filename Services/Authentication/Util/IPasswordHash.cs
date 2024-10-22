namespace QuizApp.Services.Authentication.Util
{
    public interface IPasswordHash
    {
        public string HashPassword(string password);
    }
}
