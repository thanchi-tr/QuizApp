using System.Security.Cryptography;
using System.Text;

namespace QuizApp.Services.Authentication.Util
{
    public class HMACSHA256Hash : IInfoHash
    {
        private readonly IConfiguration _config;

        public HMACSHA256Hash(IConfiguration config)
        {
            _config = config;
        }

        // this will  take care of the hasing the data store in the db
        public string Hash(string info)
        {
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]);
            using (var hmac = new HMACSHA256(key))
            {
                var infoBytes = Encoding.UTF8.GetBytes(info);
                var hashBytes = hmac.ComputeHash(infoBytes);
                return Convert.ToBase64String(hashBytes);
                
            }
        }

        public bool Verify(string raw, string authenticatedPassword)
        {
            var password = Hash(raw);
            if (password.Length != authenticatedPassword.Length) return false;

            int result = 0;
            for (int i = 0; i < password.Length; i++)
            {
                result |= password[i] ^ authenticatedPassword[i];
            }
            return result == 0; throw new NotImplementedException();
        }
    }
}
