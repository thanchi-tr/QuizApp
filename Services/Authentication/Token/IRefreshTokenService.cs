using Azure.Core;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO.External.Response;

namespace QuizApp.Services.Authentication.Token
{
    public interface IRefreshTokenService
    {
        public Task<String> GenerateRefreshToken(User user, IdeaSpaceDBContext context);
        
        public Task<TokenDTO> RefreshTokens(TokenDTO tokens, IdeaSpaceDBContext context);
    }
}
