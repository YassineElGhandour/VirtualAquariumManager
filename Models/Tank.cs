using System.ComponentModel.DataAnnotations;

namespace VirtualAquariumManager.Models
{
    public class Tank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Shape { get; set; }
        public decimal Size { get; set; }
        public WaterQuality? WaterQuality { get; set; }
        public List<MaintenanceTask> MaintenanceTasks { get; set; } = [];
        public required DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}