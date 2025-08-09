namespace VirtualAquariumManager.Models
{
    public class Fish : Animal
    {
        public Fish()
        {
            Species = Species.Fish;
        }
        public string? SubSpecies { get; set; }
        public Guid? TankId { get; set; }
        public Tank? Tank { get; set; }
        public FeedingTask? NextFeeding { get; set; }
        public List<FeedingTask>? FeedingTasks { get; set; } = [];
        public required DateTime ImportedDate { get; set; }
    }
}