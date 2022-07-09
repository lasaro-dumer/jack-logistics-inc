using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JackLogisticsInc.API.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

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

        protected async Task<List<T>> AssertOkGetOfCollection<T>(HttpResponseMessage response)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<T> parsedResponse = JsonConvert.DeserializeObject<List<T>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
            return parsedResponse;
        }

        protected async Task<T> AssertOkGetObject<T>(HttpResponseMessage response)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            T parsedResponse = JsonConvert.DeserializeObject<T>(responseContent);
            Assert.NotNull(parsedResponse);
            return parsedResponse;
        }

        protected async Task AssertBadRequest(HttpResponseMessage response)
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(response.Headers.Location);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body with further details");
        }

        protected async Task AssertCreated(HttpResponseMessage response)
        {
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
        }
    }
}