namespace TodoAPI.DbModels
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int TodoID { get; set; }
        public DateTime TimeStamp { get; set; }
        public required string Message { get; set; }

        public bool isRead { get; set; }

        public required Todo Todo { get; set; }
    }
}