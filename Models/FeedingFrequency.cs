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
        public int NumberOfTimes { get; set; }
        public UnitOfFeeding UnitOfFeeding { get; set; }
    }

}