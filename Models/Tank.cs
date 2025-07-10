namespace VirtualAquariumManager.Models
{
    public class Tank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Shape { get; set; }
        public decimal Size { get; set; }
        public required WaterQuality WaterQuality { get; set; }
        public List<MaintenanceTask> MaintenanceTasks { get; set; } = [];
    }
}