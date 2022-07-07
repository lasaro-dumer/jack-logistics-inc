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
        private readonly HttpClient _client;

        public WarehousesControllerTests()
        {
            _client = GetClient();
        }

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

        [Fact]
        public async Task ShouldGetAWarehouseLocationsByIdWhenStateDefaultsToPullFreeLocations()
        {
            // Arrange
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application,
                buildings : 1, floors : 1, corridors : 1, shelves : 2,
                addPackages : false);

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}/locations");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<Location> parsedResponse = JsonConvert.DeserializeObject<List<Location>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
            Assert.Equal(2, parsedResponse.Count);
        }

        [Fact]
        public async Task ShouldGetAWarehouseLocationsByIdWhenAskingForLocationsWithPackages()
        {
            // Arrange
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application,
                buildings : 1, floors : 1, corridors : 2, shelves : 2,
                addPackages : true);

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}/locations?locationStateFilter={LocationStateFilter.Assigned}");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<Location> parsedResponse = JsonConvert.DeserializeObject<List<Location>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
            Assert.Equal(4, parsedResponse.Count);
        }

        [Fact]
        public async Task ShouldGetAWarehouseLocationsByIdWhenAskingForLocationsInAnyState()
        {
            // Arrange
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application,
                buildings : 1, floors : 1, corridors : 1, shelves : 3,
                addPackages : false);

            ObjectMother.AddPackageToLocation(Application, warehouse.Locations.First());

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}/locations?locationStateFilter={LocationStateFilter.Any}");
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string responseContent = await ((StreamContent)response.Content).ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent), "The response must have a body");
            List<Location> parsedResponse = JsonConvert.DeserializeObject<List<Location>>(responseContent);
            Assert.NotNull(parsedResponse);
            Assert.NotEmpty(parsedResponse);
            Assert.Equal(3, parsedResponse.Count);
        }
    }
}