namespace QuizApp.Services.Authentication.Token
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using QuizApp.Model.Domain;
    using System.Security.Cryptography;
    using QuizApp.Data;
    using Microsoft.EntityFrameworkCore;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Use a Crypto safe rng to generate a Refresh key
        ///     each user will only have 1 key.
        /// </summary>
        /// <param name="user"> Domain(sql table) user</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<String> GenerateRefreshToken(User user, IdeaSpaceDBContext context)
        {
            var randomNumber = new byte[64];
                                    using (var rng = RandomNumberGenerator.Create())
                                    {
                                        rng.GetBytes(randomNumber);
                                    }
            /* @todo bring this into a repository:: add or update the refresh token
             */
            var rfToken = await context
                    .RefreshTokens
                    .FirstOrDefaultAsync(rfToken => rfToken.UserId == user.UserId);
            if (rfToken == null) // first grant
            {
                rfToken = new RefreshToken
                {
                    UserId = user.UserId,
                    IsRevoked = false,
                    Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtSettings:RefreshTokenExpirationDays"])),
                    Created = DateTime.UtcNow,
                    Token = Convert.ToBase64String(randomNumber),
                };

                context.RefreshTokens.Add(rfToken);
                await context.SaveChangesAsync();
                return rfToken.Token;
            }

            rfToken.Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtSettings:RefreshTokenExpirationDays"]));
            rfToken.Created = DateTime.UtcNow;
            rfToken.IsRevoked = false;
            rfToken.Token = Convert.ToBase64String(randomNumber);
            await context.SaveChangesAsync();
            return rfToken.Token;
        }

        /// <summary>
        /// Customise out own JWT
        /// </summary>
        /// <param name="user">SQL entity: User</param>
        /// <returns></returns>
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = _configuration["JwtSettings:Secret"];

            var claims = new List<Claim> // define the set of claim/attribute that describe user in JWT
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), // Unique name
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(JwtRegisteredClaimNames.Iat, 
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64), // Issued at
                new Claim(ClaimTypes.Role, "tester") // Role(user entity does not have role as in now) is a custome Claims that outside the JWT specification (IETF RFC 7519) @todo: implement role base authorization
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated Token: {tokenStr}");
            return tokenStr;
        }
    }

}
