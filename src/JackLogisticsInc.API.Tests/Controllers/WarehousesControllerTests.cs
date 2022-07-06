using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Tests.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace JackLogisticsInc.API.Tests.Controllers
{
    public class WarehousesControllerTests : ApiControllerTestBase
    {
        private readonly HttpClient _client;

        public WarehousesControllerTests()
        {
            _client = GetClient();
        }

        [Fact]
        public async Task ShouldListWarehouses()
        {
            // Arrange
            int warehouseId = ObjectMother.SetupWarehouseWithLocations(Application);

            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/warehouses");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<Warehouse> parsedResponse = JsonConvert.DeserializeObject<List<Warehouse>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
        }

        [Fact]
        public async Task ShouldGetAWarehouseById()
        {
            // Arrange
            int warehouseId = ObjectMother.SetupWarehouseWithLocations(Application);

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouseId}");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            Warehouse parsedResponse = JsonConvert.DeserializeObject<Warehouse>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.False(string.IsNullOrEmpty(parsedResponse.Name));
        }

        [Fact]
        public async Task ShouldGetAWarehouseLocationsById()
        {
            // Arrange
            int buildings = 1;
            int floors = 2;
            int corridors = 2;
            int shelves = 4;
            int warehouseId = ObjectMother.SetupWarehouseWithLocations(Application, buildings, floors, corridors, shelves);
            int expectedLocations = buildings * floors * corridors * shelves;

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouseId}/locations");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<Location> parsedResponse = JsonConvert.DeserializeObject<List<Location>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
            Assert.Equal(expectedLocations, parsedResponse.Count);
        }
    }
}