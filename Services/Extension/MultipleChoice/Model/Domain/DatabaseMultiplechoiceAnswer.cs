namespace QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.Domain
{
    // The format chosen to save answer of the Multichoice question in the Answer.value in SQl db
    public class DatabaseMultiplechoiceAnswer
    {
        public string Correct { get; set; }
        
        /// <summary>
        /// There will be 3 incorrect answer
        /// </summary>
        public List<string> Incorrect { get; set; }
    }
}
