using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Model.DTO.External.Resquest
{
    [SwaggerSchema(Description = "Addition on top of the HTTPS, we can use Asymmetric to encrypt this LoginDTO")]
    public class CredentialDTO
    {
        
        public string UserName { get; set; }
        public string Password { get; set; }    
    }
}
