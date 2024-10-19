namespace QuizApp.Model.DTO.External
{
    /// <summary>
    /// Data receive through a body of a https request
    /// </summary>
    public class RestfulAnswer
    {
        public string Type { get; set; }
        public string CollectionId {  get; set; }
        /// <summary>
        /// In the form of serialized( MultipleChoiceValidateAnswer)
        /// </summary>
        public string Answer {  get; set; }
    }
}
