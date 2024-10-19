using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Model.Domain
{
    // Follow the SQL convention (only use snake_case)
    public class Collection
    {
        
        [Key, Column("collection_id")]
        public Guid CollectionId { get; set; }

        public ICollection<Question> Questions { get; set; }
        [Required, Column("name")]
        public String Name { get; set; }

        [Required, Column("user_id")]
        public Guid UserId { get; set; }
        public virtual Domain.User User { get; set; }
    }   
}
