namespace QuizApp.Model.DTO.External
{
    /// <summary>
    /// use to deserialized
    /// </summary>
    public class MChoiceQuestionCreateDTO
    {
        public string Question { get; set; } // the question name
        public string Type { get; set; }
        public string Correct { get; set; }
        public List<string> Incorrect { get; set; }

        
    }
}
