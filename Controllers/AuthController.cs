﻿using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Data;
using QuizApp.Model.DTO.External.Response;
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
        private readonly IdeaSpaceDBContext _dbContext;

        public AuthController(IAuthService userService, ITokenService tokenService , IdeaSpaceDBContext context)
        {
            _userService = userService;
            _tokenService = tokenService;
            _dbContext = context;
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
            "OK:: authentication successfull, expect token to be return in payload",
            ContentTypes = new[] { "application/json" },
            Type = typeof(TokenDTO)
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
                    ? Ok(new TokenDTO{ 
                        AccessToken = _tokenService.GenerateToken(result.Data),
                        RefreshToken = await _tokenService.GenerateRefreshToken(result.Data, _dbContext)
                    }
                    ) :
                (result.Message.Contains("Missing prop")) ? BadRequest(new { message = "Missing username / password" }) :
                Unauthorized(new { message ="Fail attempt"});
        }

        [HttpPost("Register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(
            200,
            "Created:: Register successfull, expect token to be return in payload"
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



        [HttpPost("Refresh")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(
            401,
            "Un-authorize::The client missing/provide invalid credential"
        )]
        [SwaggerResponse(
            204,
            "No Content:: Log out peacefully"
        )]
        public async Task<IActionResult> RefreshAccess()
        {
            //optain the require token pairs
            if(Request.Headers.TryGetValue("RefreshToken", out var refreshToken) && Request.Headers.TryGetValue("Authorization", out var bearer))
            {
                // extract the access token from bearer
                var accessToken = bearer.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                if (string.IsNullOrEmpty(accessToken))
                {
                    return Unauthorized();
                }

                var result = await _userService.RefreshLogin(new TokenDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
                
                return 
                    (result.Status) ? Ok(result.Data):
                    Unauthorized();
            }
            return Unauthorized();
        }


        [HttpPost("Logout")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(
            400,
            "Bad Request::The client missing/provide invalid credential"
        )]
        [SwaggerResponse(
            200,
            "Success:: Expect the new pair of token in body"
        )]
        public async Task<IActionResult> LogOut()
        {
            var requestorId = User.FindFirst("Sub")?.Value ??
                            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (requestorId == null)
            {
                return BadRequest();
            }
            var result = await _userService.LogOutAsync(requestorId);

            return (result.Status) ? NoContent() :
                BadRequest();
        }
    }


     

}
