using Microsoft.AspNetCore.Mvc;
using QuizApp.Services.Authentication;
using QuizApp.Services.Authentication.Token;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("Api/Auth/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService userService, ITokenService tokenService  )
        {
            _userService = userService;
            _tokenService = tokenService;
        }
    }


}
