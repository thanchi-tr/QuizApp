using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Model.Domain
{
    // Follow the SQL convention (only use snake_case)
    public class User
    {
      

        [Key, Column("user_id")]
        public Guid UserId { get; set; }
        [Required, Column("user_name")]
        public string UserName { get; set; }

        public ICollection<Collection> Collections { get; set; }
    }
}
