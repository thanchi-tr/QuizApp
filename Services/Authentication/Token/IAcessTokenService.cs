using QuizApp.Data;
using QuizApp.Model.Domain;

namespace QuizApp.Services.Authentication.Token
{
    public interface IAccessTokenService
    {
        public string GenerateToken(User user);
    }
}
