using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Models;
using JackLogisticsInc.API.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace src.JackLogisticsInc.API.Tests.Controllers
{
    public class ShipmentsControllerTests : ApiControllerTestBase
    {
        [Fact]
        public async Task ShouldShipAPackageWhenTruckFreeAndPackagePendingOnLocation()
        {
            // Arrange
            //Ensure there are no shipments in transit
            using(var scope = Application.Services.CreateScope())
            {
                LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                var inTransit = dbContext.Shipments.Where(s => !s.DeliveredAt.HasValue);
                foreach (var shipment in inTransit)
                    shipment.DeliveredAt = DateTime.UtcNow;

                dbContext.SaveChanges();
            }

            Package package = ObjectMother.GetNewPackage(Application);
            ShipPackageModel shipPackageModel = new ShipPackageModel()
            {
                PackageId = package.Id,
                DestinationAddressData = ObjectMother.NewAddressData()
            };

            string stringPayload = JsonConvert.SerializeObject(shipPackageModel);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/shipments", httpContent);

            // Assert
            await AssertCreated(response);
            //Assert database updates
            using(var scope = Application.Services.CreateScope())
            {
                LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                var updatedPackage = dbContext.Packages.Include(p => p.Location).First(p => p.Id == package.Id);
                Assert.Null(updatedPackage.Location);

                int inTransitShipments = dbContext.Shipments.Where(s => !s.DeliveredAt.HasValue).Count();
                Assert.Equal(1, inTransitShipments);
            }
        }

        [Fact]
        public async Task ShouldFailToShipAPackageThatDoesntExists()
        {
            // Arrange
            ShipPackageModel shipPackageModel = new ShipPackageModel()
            {
                PackageId = -1,
                DestinationAddressData = ObjectMother.NewAddressData()
            };

            string stringPayload = JsonConvert.SerializeObject(shipPackageModel);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/shipments", httpContent);

            // Assert
            await AssertBadRequest(response);
        }

        [Fact]
        public async Task ShouldFailToShipAPackageWithoutDestination()
        {
            // Arrange
            Package package = ObjectMother.GetNewPackage(Application);
            ShipPackageModel shipPackageModel = new ShipPackageModel()
            {
                PackageId = package.Id,
            };

            string stringPayload = JsonConvert.SerializeObject(shipPackageModel);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/shipments", httpContent);

            // Assert
            await AssertBadRequest(response);
        }

        [Fact]
        public async Task ShouldFailToShipAPackageIfThereAreShipmentsInTransit()
        {
            // Arrange
            //Ensure there are shipments in transit
            ObjectMother.SetupInTransitShipments(Application);

            Package package = ObjectMother.GetNewPackage(Application);
            ShipPackageModel shipPackageModel = new ShipPackageModel()
            {
                PackageId = package.Id,
                DestinationAddressData = ObjectMother.NewAddressData()
            };

            string stringPayload = JsonConvert.SerializeObject(shipPackageModel);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await _client.PostAsync("/api/shipments", httpContent);

            // Assert
            await AssertBadRequest(response);
        }
    }
}