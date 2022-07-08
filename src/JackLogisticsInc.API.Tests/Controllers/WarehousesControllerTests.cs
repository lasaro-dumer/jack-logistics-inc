using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Models;
using JackLogisticsInc.API.Tests.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace JackLogisticsInc.API.Tests.Controllers
{
    public class WarehousesControllerTests : ApiControllerTestBase
    {
        [Fact]
        public async Task ShouldListWarehouses()
        {
            // Arrange
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application);

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
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application);

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}");
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
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application, buildings, floors, corridors, shelves);
            int expectedLocations = buildings * floors * corridors * shelves;

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}/locations");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<Location> parsedResponse = JsonConvert.DeserializeObject<List<Location>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
            Assert.Equal(expectedLocations, parsedResponse.Count);
        }

        [Theory]
        [InlineData(1, 2, 2, LocationStateFilter.Free)]
        [InlineData(2, 2, 4, LocationStateFilter.Assigned)]
        [InlineData(1, 3, 3, LocationStateFilter.Any)]
        public async Task ShouldGetAWarehouseLocationsByIdAccordingToTheirState(int corridors, int shelves, int expectedLocations, LocationStateFilter locationStateFilter)
        {
            // Arrange
            bool addPackages = locationStateFilter == LocationStateFilter.Assigned;

            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application,
                buildings : 1, floors : 1, corridors : corridors, shelves : shelves,
                addPackages : addPackages);

            string queryString = locationStateFilter != LocationStateFilter.Free ? $"?locationStateFilter={locationStateFilter}" : "";

            if (locationStateFilter == LocationStateFilter.Any)
                ObjectMother.AddPackageToLocation(Application, warehouse.Locations.First());

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}/locations{queryString}");
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