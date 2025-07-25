using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtualAquariumManager.Models;

namespace VirtualAquariumManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<VirtualAquariumManager.Models.Tank> Tank { get; set; } = default!;
        public DbSet<VirtualAquariumManager.Models.Fish> Fish { get; set; } = default!;
    }
}
