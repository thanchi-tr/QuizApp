using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO.Internal;
using QuizApp.Services.Authentication.Util;

namespace QuizApp.Services.Authentication
{
    public class UserService
    {

        private readonly IdeaSpaceDBContext _context;
        private readonly IPasswordHash _hasher;
        private readonly IPasswordVerificate _validator;
        private readonly IPasswordStengthValidate _stengthValidator;

        public UserService(IdeaSpaceDBContext context,
                    IPasswordHash hasher,
                    IPasswordVerificate validator,
                    IPasswordStengthValidate stengthValidate)
        {
            _context = context;
            _hasher = hasher;
            _validator = validator;
            _stengthValidator = stengthValidate;
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
            /*find the user:: @todo: abstract this logic into repository*/
            var user = await _context.Users
                                .AsNoTracking()
                                .SingleOrDefaultAsync(u => u.UserName == userName);
            if(user == null)
                return new BusinessToPresentationLayerDTO<User>(false, "Not found", null);

            var passwordHash = _hasher.HashPassword(password);


            
            if ( !_validator.Verify(passwordHash, user.HashedPassword))
                return new BusinessToPresentationLayerDTO<User>(false, "Non-Authorized", null);
            
            return new BusinessToPresentationLayerDTO<User>(true, "", user);
        }

        public async Task<BusinessToPresentationLayerDTO<User>> ResisterAsync(string userName, string password)
        {
            if(!_stengthValidator.Validate(password))
                return new BusinessToPresentationLayerDTO<User>(false, "Weak password", null);

            var passHash = _hasher.HashPassword(password);

            /*create the user::@todo: abstract this logic into repository*/
            var user = new User
            {
                UserName = userName,
                HashedPassword = passHash
            };
            await _context.SaveChangesAsync();

        }
    }
}
