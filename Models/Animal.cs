namespace VirtualAquariumManager.Models
{
    public enum Species
    {
        Mammal,
        Fish,
        Insects
    }

    public abstract class Animal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public Species Species { get; protected set; }
        public int LifeSpan { get; set; }
        public FeedingFrequency? FeedingFrequency { get; set; }
    }

}