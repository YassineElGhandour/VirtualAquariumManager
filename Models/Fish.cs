namespace VirtualAquariumManager.Models
{
    public class Fish : Animal
    {
        public Fish()
        {
            Species = Species.Fish;
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<FishTank> FishTank { get; set; } = [];
        public FeedingTask? NextFeeding { get; set; }
        public List<FeedingTask>? FeedingTasks { get; set; } = [];
    }
}