using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Services.Authentication;
using QuizApp.Services.Authentication.Token;
using Swashbuckle.AspNetCore.Annotations;

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


        [HttpPost("Login")]// potentially implement a asymetric key pair to encrypt the useName and attemptPassword for addition security
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Authenticate the user against Credentail",
            Description = "Chosen media: SQL DB to store credential"
        )]
        [SwaggerResponse(
            200,
            "OK:: authentication successfull, expect token to be return in payload"
        )]
        [SwaggerResponse(
            400,
            "Bad Request:: indicate that the payload is missing key prop"
        )]
        [SwaggerResponse(
            404,
            "Not found:: fail to authenticate the attempt"
        )]
        public async Task<IActionResult> Login(
            [SwaggerRequestBody(
                Required = true
            )]
            [FromBody] CredentialDTO attempt
        )
        {
            var result = await _userService.AuthenticateAsync(attempt.UserName, attempt.Password);

            return
                (result.Status) 
                    ? Ok(_tokenService.GenerateToken(result.Data)) :
                (result.Message.Contains("Missing prop")) ? BadRequest(new { message = "Missing username / password" }) :
                Unauthorized(new { message ="Fail attempt"});
        }

        [HttpPost("Register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(
            200,
            "OK:: Register successfull, expect token to be return in payload"
        )]
        [SwaggerResponse(
            400,
            "Bad Request:: indicate that the strength of the credential is not enough"
        )]
        public async Task<IActionResult> Register(
            [SwaggerRequestBody(
                Required = true
            )]
            [FromBody] CredentialDTO request
            )
        {
            var result = await _userService.ResisterAsync(request.UserName, request.Password);
            return
                (result.Status) ? Ok(_tokenService.GenerateToken(result.Data)) :
                BadRequest(new { message = "Fail strength test" });
        }
    }


     

}
