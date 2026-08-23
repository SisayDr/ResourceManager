using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResourceManagerAPI.Data;

namespace ResourceManagerAPI.Tests
{
    public static class TestFactory
    {
        public static AppDbContext GetDbContext() {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var httpContextAccessor = new HttpContextAccessor();
            return new AppDbContext(options, httpContextAccessor);
        }

    }
}
