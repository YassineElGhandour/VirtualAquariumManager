namespace VirtualAquariumManager.Models
{
    public class Fish : Animal
    {
        public Fish()
        {
            Species = Species.Fish;
        }
        public required string SubSpecies { get; set; }
        public List<FishTank> FishTank { get; set; } = [];
        public FeedingTask? NextFeeding { get; set; }
        public List<FeedingTask>? FeedingTasks { get; set; } = [];
        public required DateTime ImportedDate { get; set; }
    }
}