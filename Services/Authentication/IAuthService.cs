using QuizApp.Model.Domain;
using QuizApp.Model.DTO.External.Response;
using QuizApp.Model.DTO.Internal;

namespace QuizApp.Services.Authentication
{
    public interface IAuthService
    {
        public Task<BusinessToPresentationLayerDTO<User>> AuthenticateAsync(string userName, string password);
        public Task<BusinessToPresentationLayerDTO<User>> ResisterAsync(string userName, string password);
        public Task<BusinessToPresentationLayerDTO<TokenDTO>> RefreshLogin(TokenDTO tokenPair);

        public Task<BusinessToPresentationLayerDTO<string>> LogOutAsync(string userId);
    }
}
