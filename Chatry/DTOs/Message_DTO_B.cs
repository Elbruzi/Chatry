namespace Chatry.DTOs
{
    public class Message_DTO_B
    {
        public int UserID { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; } = true;

        public int RoomID { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    }
}
