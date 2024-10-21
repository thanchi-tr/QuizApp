using NHibernate.Criterion;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Model.DTO.External
{
    /// <summary>
    /// use to deserialized
    /// </summary>
    public class MultipleChoiceQuestionAnswerDTO
    {
        [SwaggerSchema(Description = "The Question sentence")]
        public string Question { get; set; } // the question name

        [SwaggerSchema(Description = "Type of question: used to parse, process this quesiton. Seek Format api to get to know the format")]
        public string Type { get; set; }
        public string Correct { get; set; }
        public List<string> Incorrect { get; set; }

        
    }
}
