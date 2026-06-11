using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatry.Models
{
    public class Rooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID { get; set; }

        public string RoomName { get; set; }

        public int FriendStatus { get; set; } = 0;

        public ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
    }
}
