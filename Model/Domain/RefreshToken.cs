using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.Model.Domain
{
    public class RefreshToken
    {
        [Key, Column("token_id")]
        public Guid RefreshTokenId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime Expires { get; set; }
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsRevoked { get; set; }

        [ForeignKey("User"),Required]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
