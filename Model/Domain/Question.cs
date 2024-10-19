using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Model.Domain
{
    // Follow the SQL convention (only use snake_case)
    public class Question
    {
        [Key,Column("question_id")]
        public Guid QuestionId { get; set; }

        [Column("collection_id")]
        public Guid CollectionId { get; set; }

        [Required]
        public virtual Domain.Collection Collection { get; set; }
        
        public Domain.Answer Answer { get; set; }
        public Domain.Level Level { get; set; }

        [Required, Column("value")]
        public required string Value { get; set; }
        [Required, Column("last_revision")]
        public DateOnly LastRevision { get; set; }
    }
}
