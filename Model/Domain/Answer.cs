using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuizApp.Model.Domain
{
    public class Answer
    {
        [Key, Column("answer_id")]
        public Guid QuestionId { get; set; }

        [JsonIgnore]
        public virtual Question Question { get; set; }

        [Required, Column("value")]
        public required String Value { get; set; }
        [Required, Column("meta_answer_type")]
        public String Type { get; set; }
        public virtual MetaAnswer MetaAnswer { get; set; }
    }
}
