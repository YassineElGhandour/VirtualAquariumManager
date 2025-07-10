namespace VirtualAquariumManager.Models
{
    public class Fish : Animal
    {
        public Fish()
        {
            Species = Species.Fish;
        }

        public Tank Tank { get; set; }
        public FeedingTask NextFeeding { get; set; }
        public List<FeedingTask> FeedingTasks { get; set; } = [];
    }
}
