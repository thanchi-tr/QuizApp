using QuizApp.Model.Domain;

namespace QuizApp.Services.Authentication.Token
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}
