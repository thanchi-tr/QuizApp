using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Model.Domain
{
    // Follow the SQL convention (only use snake_case)
    public class MetaAnswer
    {

        [Key, Column("type")]
        public required String Type {  get; set; }
        [Column("test_service")]
        public string TestServiceType { get; set; }
        [Column("answer_service")]
        public string AnswerServiceType { get; set; }
        [Column("validation_service")]
        public string ValidationServiceType { get; set; }

        public ICollection<Answer> Answers { get; set; }

    }
}
