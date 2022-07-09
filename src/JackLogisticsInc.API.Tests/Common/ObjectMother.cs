using System;
using System.Linq;
using JackLogisticsInc.API;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JackLogisticsInc.API.Tests.Common
{
    public static class ObjectMother
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Warehouse SetupWarehouseWithLocations(WebApplicationFactory<Program> application,
            int buildings = 5, int floors = 2,
            int corridors = 4, int shelves = 8,
            bool addPackages = false)
        {
            using(var scope = application.Services.CreateScope())
            {
                return SetupWarehouseWithLocations(scope, buildings, floors, corridors, shelves, addPackages);
            }
        }

        public static Warehouse SetupWarehouseWithLocations(IServiceScope scope,
            int buildings = 5, int floors = 2,
            int corridors = 4, int shelves = 8,
            bool addPackages = false)
        {
            Warehouse warehouse = new Warehouse()
            {
            Name = $"Deposit {RandomString(10)}"
            };

            ScopedAddWarehouse(scope, buildings, floors, corridors, shelves, addPackages, warehouse);

            return warehouse;
        }

        private static void ScopedAddWarehouse(IServiceScope scope, int buildings, int floors, int corridors, int shelves, bool addPackages, Warehouse warehouse)
        {
            LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

            for (int b = 0; b < buildings; b++)
            {
                string buildingName = RandomString(4);

                for (int f = 0; f < floors; f++)
                {
                    for (int c = 0; c < corridors; c++)
                    {
                        for (int s = 0; s < shelves; s++)
                        {
                            Location location = new Location()
                            {
                                Building = buildingName,
                                Floor = f.ToString(),
                                Corridor = $"C{c}",
                                Shelf = s.ToString().PadLeft(4, '0')
                            };

                            if (addPackages)
                                location.Package = NewPackage();

                            warehouse.Locations.Add(location);
                        }
                    }
                }
            }

            dbContext.Warehouses.Add(warehouse);
            dbContext.SaveChanges();
        }

        public static Location GetLocation(WebApplicationFactory<Program> application, bool free)
        {
            using(var scope = application.Services.CreateScope())
            {
                LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                Location location = dbContext.Locations
                    .Include(l => l.Package)
                    .FirstOrDefault(l => free ? l.Package == null : l.Package != null);

                if (location != null)
                    return location;
            }

            //Didn't find any location, add a warehouse with a location and using it
            return SetupWarehouseWithLocations(application, 1, 1, 1, 1, !free).Locations.First();
        }

        public static Location GetFreeLocation(WebApplicationFactory<Program> application)
        {
            return GetLocation(application, free : true);
        }

        public static Location GetOccupiedLocation(WebApplicationFactory<Program> application)
        {
            return GetLocation(application, free : false);
        }

        public static Package NewPackage()
        {
            return new Package()
            {
                Description = $"A package with {RandomString(20)}"
            };
        }

        public static string NewAddressData()
        {
            return $"Street {RandomString(5)}";
        }

        public static void AddPackageToLocation(WebApplicationFactory<Program> application, Location location)
        {
            using(var scope = application.Services.CreateScope())
            {
                LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                dbContext.Locations.First(l => l.Id == location.Id).Package = NewPackage();

                dbContext.SaveChanges();
            }
        }

        public static Package GetNewPackage(WebApplicationFactory<Program> application)
        {
            Warehouse warehouse = SetupWarehouseWithLocations(application, 1, 1, 1, 1, true);
            return warehouse.Locations.First().Package;
        }

        public static Package GetNewPackage(IServiceScope scope)
        {
            Warehouse warehouse = SetupWarehouseWithLocations(scope, 1, 1, 1, 1, true);
            return warehouse.Locations.First().Package;
        }

        public static void SetupInTransitShipments(WebApplicationFactory<Program> application)
        {
            using(var scope = application.Services.CreateScope())
            {
                LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                if (dbContext.Shipments.Any())
                {
                    var inTransit = dbContext.Shipments.Where(s => s.DeliveredAt.HasValue);
                    //I know we only need one in transit, but let's update all for simplicity and assertiviness
                    foreach (var shipment in inTransit)
                        shipment.DeliveredAt = null;

                }
                else
                {
                    //there were no shipment when the test run (parallel or out of order tests can be tricky), so let's add a shipment in transit
                    Package package = GetNewPackage(scope);

                    Shipment shipment = new Shipment()
                    {
                        DestinationAddress = NewAddressData(),
                        LeftForDestinationAt = DateTime.UtcNow,
                    };
                    shipment.Packages.Add(package);

                    package.Location = null;

                    dbContext.Shipments.Add(shipment);
                }

                dbContext.SaveChanges();
            }
        }
    }
}