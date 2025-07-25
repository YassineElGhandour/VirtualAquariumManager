namespace VirtualAquariumManager.Models
{
    public class FishTank
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FishId { get; set; }
        public Fish Fish { get; set; } = null!;
        public Guid TankId { get; set; }
        public Tank Tank { get; set; } = null!;
        public DateTime AssignedDate { get; set; }
    }
}