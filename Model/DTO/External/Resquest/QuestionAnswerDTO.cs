using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Model.DTO.External.Resquest
{
    
    public class QuestionAnswerDTO
    {
        [SwaggerSchema(Description = "The Question sentence")]
        public string? Question { get; set; } // the question name

        [SwaggerSchema(Description = "Type of question: used to parse, process this quesiton. Seek Format api to get to know the format")]
        public string? Type { get; set; }

        [SwaggerSchema(Description = "the string represent the actual answer object according to its type")]
        public string? SerializedAnswer { get; set; }
    }
}
