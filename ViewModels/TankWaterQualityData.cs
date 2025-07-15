using VirtualAquariumManager.Models;

namespace VirtualAquariumManager.ViewModels
{
    public class TankWaterQualityData
    {
        public required Guid TankId { get; set; }
        public required string Shape { get; set; }
        public required decimal Size { get; set; }
        public required WaterQuality WaterQuality { get; set; }
        public required decimal PhLevel { get; set; }
        public required decimal Temperature { get; set; }
        public decimal? AmmoniaLevel { get; set; }
        public string? WaterType { get; set; }
        public required DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}