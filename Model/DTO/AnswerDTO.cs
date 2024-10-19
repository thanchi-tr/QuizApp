using QuizApp.Model.Domain;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Model.DTO
{
    public class AnswersDTO
    {
        public required String Title { get; set; }
        [SwaggerSchema(Description = "Concrete supported question format refer to API/Quiz/Get/Question/Format")]

        public List<String> Answer { get; set; } = new List<String>();
    }
}
