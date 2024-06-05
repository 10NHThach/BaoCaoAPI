namespace MVCImage.Models
{
    public class History
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string Entity { get; set; }
        public int EntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
