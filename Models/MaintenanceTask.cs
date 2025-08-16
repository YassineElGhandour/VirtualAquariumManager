namespace VirtualAquariumManager.Models
{
    public enum MaintenanceType
    {
        WaterChange,
        FilterSwap,
        QualityCheck
    }

    public class MaintenanceTask : AquariumTask
    {
        public required MaintenanceType Type { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
