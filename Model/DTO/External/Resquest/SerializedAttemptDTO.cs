using QuizApp.Model.Domain;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace QuizApp.Model.DTO.External.Resquest
{
    /// <summary>
    /// Data receive through a body of a https request
    /// </summary>
    public class SerializedAttemptDTO
    {
        [SwaggerSchema(Description ="Type of the question, refer to the Format Api for the supported type")]
        public string Type { get; set; }
        /// <summary>
        /// In the form of serialized( MultipleChoiceValidateAnswer)
        /// </summary>
        [SwaggerSchema(Description ="An abstraction: where the object is being serialized into a string, actual type is dictate by the Type")]

        public string Answer { get; set; }

        /// <summary>
        /// Check for every property contain some thing
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !(string.IsNullOrEmpty(Answer) || string.IsNullOrEmpty(Type));
        }
    }
}
