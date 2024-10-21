using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Model.DTO.External
{
    /// <summary>
    /// Data that will be pass to front end on request for quiz
    /// </summary>
    public class TestDTO
    {
        [SwaggerSchema(Description = "The collection ID")]
        public string CollectionId {  get; set; }
        public required string Title { get; set; }
        [SwaggerSchema(Description = "Concrete supported question format refer to API/Quiz/Get/Question/Format")]

        public List<string> Questions { get; set; } = new List<string>();
    }
}
