using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatry.Models
{
    public class Message
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }


        [Required]
        public int UserID { get; set; }


        [MaxLength(10000)]
        public required string Text { get; set; }


        public bool Active { get; set; }

       
        public required User User { get; set; }

    }
}
