using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Models;
using JackLogisticsInc.API.Tests.Common;
using Newtonsoft.Json;
using Xunit;

namespace JackLogisticsInc.API.Tests.Controllers
{
    public class PackagesControllerTests : ApiControllerTestBase
    {
        [Fact]
        public async Task ShouldListPackages()
        {
            // Arrange
            Warehouse fullWarehouse = ObjectMother.SetupWarehouseWithLocations(Application, addPackages : true);

            // Act
            HttpResponseMessage response = await _client.GetAsync("/api/packages");
            // Assert
            await AssertOkGetOfCollection<Package>(response);
        }

        [Fact]
        public async Task ShouldAddANewPackageToTheInventoryOnAFreeLocation()
        {
            // Arrange
            AddPackageModel newPackage = new AddPackageModel()
            {
                Description = ObjectMother.RandomString(20),
                LocationId = ObjectMother.GetFreeLocation(Application).Id
            };

            string stringPayload = JsonConvert.SerializeObject(newPackage, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/packages", httpContent);

            // Assert
            await AssertCreated(response);
        }

        [Fact]
        public async Task ShouldFailToAddANewPackageToTheInventoryOnAOccupiedLocation()
        {
            // Arrange
            AddPackageModel newPackage = new AddPackageModel()
            {
                Description = ObjectMother.RandomString(20),
                LocationId = ObjectMother.GetOccupiedLocation(Application).Id
            };

            string stringPayload = JsonConvert.SerializeObject(newPackage, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/packages", httpContent);

            // Assert
            await AssertBadRequest(response);
        }

        [Fact]
        public async Task ShouldFailToAddANewPackageToTheInventoryOnAInvalidLocation()
        {
            // Arrange
            AddPackageModel newPackage = new AddPackageModel()
            {
                Description = ObjectMother.RandomString(20),
                LocationId = -1
            };

            string stringPayload = JsonConvert.SerializeObject(newPackage, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/packages", httpContent);

            // Assert
            await AssertBadRequest(response);
        }
    }
}