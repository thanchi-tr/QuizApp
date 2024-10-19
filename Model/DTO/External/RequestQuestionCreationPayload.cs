namespace QuizApp.Model.DTO.External
{
    /// <summary>
    /// use to deserialized
    /// </summary>
    public class RequestQuestionCreationPayload
    {
        public string Question { get; set; } // the question name
        public string Type { get; set; }
        public string SerializedAnswer { get; set; }

        
    }
}
