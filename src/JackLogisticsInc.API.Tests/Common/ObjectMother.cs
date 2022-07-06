using System;
using System.Linq;
using JackLogisticsInc.API;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
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

        public static int SetupWarehouseWithLocations(WebApplicationFactory<Program> application,
            int buildings = 5, int floors = 2,
            int corridors = 4, int shelves = 8)
        {
            int warehouseId = 0;

            using(var scope = application.Services.CreateScope())
            {
                LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                Warehouse warehouse = new Warehouse()
                {
                    Name = $"Deposit {RandomString(10)}"
                };

                for (int b = 0; b < buildings; b++)
                {
                    string buildingName = RandomString(4);

                    for (int f = 0; f < floors; f++)
                    {
                        for (int c = 0; c < corridors; c++)
                        {
                            for (int s = 0; s < shelves; s++)
                            {
                                warehouse.Locations.Add(
                                    new Location()
                                    {
                                        Building = buildingName,
                                            Floor = f.ToString(),
                                            Corridor = $"C{c}",
                                            Shelf = s.ToString().PadLeft(4, '0')
                                    });
                            }
                        }
                    }
                }

                dbContext.Warehouses.Add(warehouse);
                dbContext.SaveChanges();
                warehouseId = warehouse.Id;
            }

            return warehouseId;
        }
    }
}