namespace VirtualAquariumManager.Models
{
    public class Alert
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string? Reason { get; set; }
        public required DateTime ScheduledAt { get; set; }
    }
}
