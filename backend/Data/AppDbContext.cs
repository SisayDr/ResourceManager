using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Models;
using System.Security.Claims;

namespace ResourceManagerAPI.Data{
    public class AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<User> (options){
        public DbSet<Group> Groups { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        //set auditable properties while saving db changes
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditableProps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private async void SetAuditableProps(){
            string? userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            userId = userId is not null ? userId: "UNKOWN";
            foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>()){
                if (entry.State == EntityState.Added) entry.Entity.SetCreated(userId);
                else if (entry.State == EntityState.Modified) entry.Entity.SetModified(userId);
            }
        }
    }
}
