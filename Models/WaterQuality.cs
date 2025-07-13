namespace VirtualAquariumManager.Models
{
    public class WaterQuality
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal PhLevel { get; set; }
        public decimal Temperature { get; set; }
        public decimal AmmoniaLevel { get; set; }
        public string? WaterType { get; set; }
        public required DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
