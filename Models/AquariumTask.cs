namespace VirtualAquariumManager.Models
{
    public abstract class AquariumTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public List<Alert> Alerts { get; set; } = [];
    }
}
