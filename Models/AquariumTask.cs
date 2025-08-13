namespace VirtualAquariumManager.Models
{
    public abstract class AquariumTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Guid TankId { get; set; }
        public required Tank Tank { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public List<Alert> Alerts { get; set; } = [];
    }
}
