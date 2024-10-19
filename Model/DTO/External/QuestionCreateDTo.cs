namespace QuizApp.Model.DTO.External
{
    /// <summary>
    /// use to deserialized
    /// </summary>
    public class QuestionCreateDTO
    {
        public string CollectionId { get; set; }
        public string Value { get; set; } // the question name
        public string Type { get; set; }
        public String SerializedAnswer { get; set; }
    }
}
