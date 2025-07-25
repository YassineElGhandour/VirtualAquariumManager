namespace VirtualAquariumManager.Models
{
    public enum UnitOfFeeding
    {
        Hour,
        TwoHours,
        FourHours,
        TenHours,
        Daily
    }

    public class FeedingFrequency
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int NumberOfTimes { get; set; }
        public UnitOfFeeding UnitOfFeeding { get; set; }
    }

}