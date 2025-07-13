namespace VirtualAquariumManager.Models
{
    public class WaterQuality
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required decimal PhLevel { get; set; }
        public required decimal Temperature { get; set; }
        public decimal? AmmoniaLevel { get; set; }
        public required string WaterType { get; set; }
        public required DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}