using Azure.Core;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO.External.Response;
using QuizApp.Model.DTO.Internal;
using QuizApp.Services.Authentication.Token;
using QuizApp.Services.Authentication.Util;

namespace QuizApp.Services.Authentication
{
    public class UserService : IAuthService
    {

        private readonly IdeaSpaceDBContext _context;
        private readonly IHash _hasher;
        private readonly IValidate<String> _stengthValidator;
        private readonly IValidate<User> _userNameValidator;
        private readonly ITokenService _tokenService;

        public UserService(IdeaSpaceDBContext context,
                            IHash hasher,
                            IValidate<string> stengthValidator, 
                            IValidate<User> userNameValidator,
                            ITokenService tokenService)
        {
            _context = context;
            _hasher = hasher;
            _stengthValidator = stengthValidator;
            _userNameValidator = userNameValidator;
            _tokenService = tokenService;
        }







        /// <summary>
        /// Securely check if the user exist
        /// 
        ///     Current exploitable area: (1) use symmetric key to sign it
        ///                               (2) the token does support encryption/decryption (potentially encrypt it using Asymmetric on the client to send)
        ///                               
        ///     potential improvement:
        ///         (1) address the 2 point above
        ///         (2) Logging and impose a overseeing function <- if the over head cost is justify
        ///         (3) Key rotation::
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<BusinessToPresentationLayerDTO<User>> AuthenticateAsync(string userName, string password)
        {

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return new BusinessToPresentationLayerDTO<User>(false, "Missing prop", null);

            /* It is best practise to keep track of attempt and temporarily block this route and user for security*/

            /*find the user:: @todo: abstract this logic into repository*/
            var user = await _context.Users
                                .AsNoTracking()
                                .SingleOrDefaultAsync(u => u.UserName == userName);
            if(user == null)
                return new BusinessToPresentationLayerDTO<User>(false, "Not found", null);
            /*find the user:: @todo: abstract this logic into repository*/



            
            if ( !_hasher.Verify(password, user.HashedPassword))
                return new BusinessToPresentationLayerDTO<User>(false, "Non-Authorized", null);
            
            return new BusinessToPresentationLayerDTO<User>(true, "", user);
        }

        /// <summary>
        /// Use 
        ///     1. Password strength validator
        ///     2. Username validator
        /// if both of the data pass the validators then proceed to create the object
        /// 22/10/24 @todo: abstract the db interaction into repository
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<BusinessToPresentationLayerDTO<User>> ResisterAsync(string userName, string password)
        {
            if(!_stengthValidator.Validate(password))
                return new BusinessToPresentationLayerDTO<User>(false, "Weak password", null);
            if(!_userNameValidator.Validate(userName))
                return new BusinessToPresentationLayerDTO<User>(false, "In appriate username", null);

            var passHash = _hasher.Hash(password);

            /*create the user::@todo: abstract this logic into repository*/
            var user = new User
            {
                UserName = userName,
                HashedPassword = passHash
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            /*create the user::@todo: abstract this logic into repository*/

            return new BusinessToPresentationLayerDTO<User>(true, "", user);

        }


        public async Task<BusinessToPresentationLayerDTO<TokenDTO>> RefreshLogin(TokenDTO tokenPair)
        {
            var newTokens = await _tokenService.RefreshTokens(tokenPair, _context);

            if(string.IsNullOrEmpty(newTokens.AccessToken))
                return new BusinessToPresentationLayerDTO<TokenDTO>(
                    false,
                    "Non-Authorized",
                    new TokenDTO { AccessToken = "", RefreshToken = "" }
                );

            return new BusinessToPresentationLayerDTO<TokenDTO>(
                    true,
                    "",
                    newTokens);
        }



        /// <summary>
        /// Revolk the Refresh token (there still be risk of the JWT be use - ignore it for now)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<BusinessToPresentationLayerDTO<string>> LogOutAsync(string userId)
        {
            if(string.IsNullOrEmpty(userId))
                return new BusinessToPresentationLayerDTO<string>
                (
                    false,
                    "Bad request",
                    ""
                );
            var rfToken = await _context.RefreshTokens
                    .FirstAsync(token => token.UserId.Equals(new Guid(userId)));
            if (rfToken != null)
            {
                rfToken.IsRevoked = true;
                await _context.SaveChangesAsync(); 
                return new BusinessToPresentationLayerDTO<string>
                (
                    true,
                    "",
                    ""
                );
            }
            return new BusinessToPresentationLayerDTO<string>
                (
                    false,
                    "Bad request",
                    ""
                );
        }
    }
}
