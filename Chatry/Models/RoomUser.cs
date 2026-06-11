using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatry.Models
{
    public class RoomUser
    {
        [Required]
        public int RoomID { get; set; }

        [Required]
        public required int UserID { get; set; }

        public  User User { get; set; }

        public  Rooms Room { get; set; }
    }
}
