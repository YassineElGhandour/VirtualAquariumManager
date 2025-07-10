namespace VirtualAquariumManager.Models
{
    public class WaterQuality
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public float PhLevel { get; set; }
        public float Temperature { get; set; }
        public float AmmoniaLevel { get; set; }
        public string? WaterType { get; set; }
    }
}
