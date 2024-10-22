using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO.Internal;
using QuizApp.Services.Authentication.Util;

namespace QuizApp.Services.Authentication
{
    public class UserService : IAuthService
    {

        private readonly IdeaSpaceDBContext _context;
        private readonly IPasswordHash _hasher;
        private readonly IPasswordVerificate _validator;
        private readonly IValidate<String> _stengthValidator;
        private readonly IValidate<User> _userNameValidator;

        public UserService(IdeaSpaceDBContext context,
                            IPasswordHash hasher, 
                            IPasswordVerificate validator,
                            IValidate<String> stengthValidator,
                            IValidate<User> userNameValidator)
        {
            _context = context;
            _hasher = hasher;
            _validator = validator;
            _stengthValidator = stengthValidator;
            _userNameValidator = userNameValidator;
        }






        /// <summary>
        /// Securely check if the user exist
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



            
            if ( !_validator.Verify(password, user.HashedPassword))
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

            var passHash = _hasher.HashPassword(password);

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
    }
}
