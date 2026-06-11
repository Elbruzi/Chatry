using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatry.Models
{
    public class Message
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  int MessageID { get; set; }

        [Required]
        public required int RoomID { get; set; }


        [Required]
        public required int UserID { get; set; }


        [MaxLength(10000)]
        public required string Text { get; set; }

        [Required]
        public required DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [Required]
        public required bool Active { get; set; } = true;

        public  User User { get; set; }

        public  Rooms Room{ get; set; }


    }
}
