using System.Collections.Generic;
using System.Linq;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JackLogisticsInc.API.Data.Repositories
{
    public class WarehouseRepository
    {
        public LogisticsDbContext DbContext { get; }

        public WarehouseRepository(LogisticsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Location GetLocationById(int locationId)
        {
            return DbContext.Locations
                .Include(l => l.Package)
                .FirstOrDefault(l => l.Id == locationId);
        }

        public List<Warehouse> GetAllWarehouses()
        {
            return DbContext.Warehouses.ToList();
        }

        public List<Location> GetWarehouseFreeLocations(int id)
        {
            return DbContext.Locations
                .Include(l => l.Package)
                .Where(l => l.WarehouseId == id &&
                    l.Package == null).ToList();
        }

        public List<Location> GetWarehouseOccupiedLocations(int id)
        {
            return DbContext.Locations
                .Include(l => l.Package)
                .Where(l => l.WarehouseId == id &&
                    l.Package != null).ToList();
        }

        public List<Location> GetWarehouseLocations(int id)
        {
            return DbContext.Locations
                .Include(l => l.Package)
                .Where(l => l.WarehouseId == id).ToList();
        }

        public Warehouse GetWarehouseById(int id)
        {
            return this.DbContext.Warehouses.First(w => w.Id == id);
        }

    }
}