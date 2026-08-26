using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResourceManagerAPI.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResourceManagerAPI.Tests.Helpers
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { Converters = { new JsonStringEnumConverter() } };

        private readonly CustomWebApplicationFactory _factory;
        private WebApplicationFactory<Program> _appFactory = null!;
        private SqlConnection _connection = null!;
        private DbTransaction _transaction = null!;
        protected HttpClient Client = null!;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }
        public async Task InitializeAsync()
        {
            var connectionString = _factory.Services.GetRequiredService<IConfiguration>()
                .GetConnectionString("DefaultConnection");

            _connection = new SqlConnection(connectionString);
            await _connection.OpenAsync();
            _transaction = await _connection.BeginTransactionAsync();

            _appFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<AppDbContext>>();

                    services.AddScoped(sp =>
                    {
                        var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseSqlServer(_connection)
                            .Options;

                        var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                        var context = new AppDbContext(options, httpContextAccessor);
                        context.Database.UseTransaction(_transaction);
                        return context;
                    });
                });
            });

            Client = _appFactory.CreateClient();
        }
        public async Task DisposeAsync()
        {
            await _transaction.RollbackAsync();
            await _connection.CloseAsync();
            Client.Dispose();
        }

        protected void ActAs(string role, string? userId = null)
        {
            Client.DefaultRequestHeaders.Remove("X-Test-User-Role");
            Client.DefaultRequestHeaders.Remove("X-Test-User-Id");

            Client.DefaultRequestHeaders.Add("X-Test-User-Role", role);
            if (userId is not null)
                Client.DefaultRequestHeaders.Add("X-Test-User-Id", userId);
        }

        protected IServiceScope GetTestScope() => _appFactory.Services.CreateScope();
    }
}
