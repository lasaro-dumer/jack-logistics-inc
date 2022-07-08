using System.Linq;
using System.Net.Http;
using JackLogisticsInc.API.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JackLogisticsInc.API.Tests.Common
{
    public class ApiControllerTestBase
    {
        public WebApplicationFactory<Program> Application { get; private set; }

        protected readonly HttpClient _client;

        public ApiControllerTestBase()
        {
            Application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType ==
                            typeof(DbContextOptions<LogisticsDbContext>));

                        services.Remove(descriptor);

                        services.AddDbContext<LogisticsDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        });
                    });
                });

            _client = Application.CreateClient();
        }
    }
}