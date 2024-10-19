namespace QuizApp.Model.DTO
{
    public class QuestionDTO
    {
        public Guid QuestionID { get; set; }
        public string Value { get; set; }
        public int Level { get; set; }
        public String Format { get; set; }
        public DateOnly LastRevision { get; set; }
    }
}
