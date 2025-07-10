namespace VirtualAquariumManager.Models
{
    public enum Role
    {
        Manager,
        Maintainer,
        Expert,
        Inspector
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public required Role Role { get; set; }
    }
}
