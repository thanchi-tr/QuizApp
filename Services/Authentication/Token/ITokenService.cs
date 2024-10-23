using QuizApp.Data;
using QuizApp.Model.Domain;

namespace QuizApp.Services.Authentication.Token
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
        public  Task<String> GenerateRefreshToken(User user, IdeaSpaceDBContext context);
    }
}
