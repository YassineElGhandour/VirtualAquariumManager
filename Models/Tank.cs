namespace VirtualAquariumManager.Models
{
    public class Tank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Shape { get; set; }
        public required decimal Size { get; set; }
        public List<FishTank> FishTank { get; set; } = [];
        public required WaterQuality WaterQuality { get; set; }
        public List<MaintenanceTask>? MaintenanceTasks { get; set; } = [];
        public required DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}