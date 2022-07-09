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
            await AssertOkGetOfCollection<Warehouse>(response);
        }

        [Fact]
        public async Task ShouldGetAWarehouseById()
        {
            // Arrange
            Warehouse warehouse = ObjectMother.SetupWarehouseWithLocations(Application);

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/warehouses/{warehouse.Id}");
            // Assert
            Warehouse returnedObject = await AssertOkGetObject<Warehouse>(response);
            Assert.False(string.IsNullOrEmpty(returnedObject.Name));
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
            List<Location> collection = await AssertOkGetOfCollection<Location>(response);
            Assert.Equal(expectedLocations, collection.Count);
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
            List<Location> collection = await AssertOkGetOfCollection<Location>(response);
            Assert.Equal(expectedLocations, collection.Count);
        }
    }
}