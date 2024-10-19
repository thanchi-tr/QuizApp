namespace QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO
{
    // The format chosen to use (store in cache, use to compare)
    public class MultipleChoiceAnswerDTO 
    {
        public string QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
