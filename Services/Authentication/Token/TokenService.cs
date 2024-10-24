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
    using QuizApp.Model.DTO.External.Response;
    using QuizApp.Services.Authentication.Util;

    public class TokenService : ITokenService
    {
        private const string INVALID_TOKEN = "";
        private IInfoHash _infoHash;
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration, IInfoHash infoHash)
        {
            _configuration = configuration;
            _infoHash = infoHash;
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
            var rawRfToken = Convert.ToBase64String(randomNumber).Replace("==", "");
            rfToken.Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtSettings:RefreshTokenExpirationDays"]));
            rfToken.Created = DateTime.UtcNow;
            rfToken.IsRevoked = false;
            rfToken.Token = _infoHash.Hash(rawRfToken); // ofer extra security incase data leakage see the configure to see the actual hashing algo in place
            await context.SaveChangesAsync();
            return rawRfToken;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<TokenDTO> RefreshTokens(TokenDTO tokens, IdeaSpaceDBContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // check the token
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = _configuration["JwtSettings:Secret"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); // use the same key for encrypt + decrypt
                var validationParameters = new TokenValidationParameters
                {
                    // Provide the key or certificate needed for decryption
                    IssuerSigningKey = key,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,  // Since it's expired
                };
                var ExpiredAccessToken = tokenHandler // extract the token only if it satisfy the validation above
                                            .ValidateToken(tokens.AccessToken,
                                                            validationParameters,
                                                            out SecurityToken validatedToken);



                // Match the refresh token as one in data base.
                var userRfToken = await context.RefreshTokens
                                            .AsNoTracking()
                                            .Include(rfToken => rfToken.User)
                                            .FirstAsync(rfToken =>
                                                rfToken.UserId == new Guid(ExpiredAccessToken.FindFirstValue("Sub") ??
                                                                ExpiredAccessToken.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                                                )
                                            );

                if(userRfToken == null ||// The user_id in the accessToken is invalid
                    userRfToken.IsRevoked == true || // refresh token is invalid due to userlog out

                    //                raw token                hash token
                    !_infoHash.Verify(tokens.RefreshToken, userRfToken.Token) ) // invalid refresh token due to miss match
                    return new TokenDTO
                    {
                        AccessToken = "",
                        RefreshToken = ""
                    };

                return new TokenDTO
                {
                    AccessToken = GenerateToken(userRfToken.User),
                    RefreshToken = await GenerateRefreshToken(userRfToken.User, context)
                };
            }
            catch (Exception ex)
            {
                // missing or incompatible token
                Console.WriteLine("Add some Log to tell the repquest try to refresh token with a invalid AccessToken");
            }

            return new TokenDTO
            {
                AccessToken ="",
                RefreshToken=""
            };
        }
    
        


    }


}
