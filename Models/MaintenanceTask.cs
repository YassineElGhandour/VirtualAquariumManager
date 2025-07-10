namespace VirtualAquariumManager.Models
{
    public enum MaintenanceType
    {
        WaterChange,
        FilterSwap,
        QualityCheck
    }

    public class MaintenanceTask : Task
    {
        public MaintenanceType Type { get; set; }
        public DateTime PerformedOn { get; set; }
    }
}
