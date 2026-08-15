using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Models;

namespace ResourceManagerAPI.Data{
    public class AppDbContext : DbContext{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Group> Groups { get; set; }
    }
}
