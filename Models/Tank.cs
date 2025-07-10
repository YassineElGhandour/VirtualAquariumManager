namespace VirtualAquariumManager.Models
{
    public class Tank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Shape { get; set; }
        public float Size { get; set; }
        public WaterQuality? WaterQuality { get; set; }
        public List<MaintenanceTask> MaintenanceTasks { get; set; } = [];
    }
}