using Azure.Core;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Model.DTO.External.Response
{
    public class TokenDTO
    {
        [SwaggerSchema(Description = "JWToken use to get access to Authorized Resource")]
        public string AccessToken {  get; set; }
        [SwaggerSchema(Description = "JWToken use to get new Access token (short live)")]
        public string RefreshToken  { get; set; }
    }
}
