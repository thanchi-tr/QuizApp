using QuizApp.Model.Domain;

namespace QuizApp.Model.DTO.External.Resquest
{
    /// <summary>
    /// Data receive through a body of a https request
    /// </summary>
    public class SerializedAttemptDTO
    {
        public string Type { get; set; } 
        /// <summary>
        /// In the form of serialized( MultipleChoiceValidateAnswer)
        /// </summary>
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
