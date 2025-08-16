namespace VirtualAquariumManager.Models
{
    public abstract class AquariumTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? TankId { get; set; }
        public Tank? Tank { get; set; }
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public required bool IsCompleted { get; set; }
        public List<Alert> Alerts { get; set; } = [];
    }
}