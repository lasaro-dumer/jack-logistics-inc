using System.Collections.Generic;
using System.Linq;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JackLogisticsInc.API.Data.Repositories
{
    public class PackagesRepository
    {
        public LogisticsDbContext DbContext { get; }

        public PackagesRepository(LogisticsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public List<Package> GetPackagesList()
        {
            return DbContext.Packages
                .Include(p => p.Location)
                .Include(p => p.Shipment)
                .ToList();
        }

        public Package GetPackageById(int id)
        {
            return DbContext.Packages
                .Include(p => p.Location)
                .Include(p => p.Shipment)
                .FirstOrDefault(p => p.Id == id);
        }

        public Package AddPackage(string description, Location location)
        {
            Package newPackage = new Package()
            {
                Description = description,
                Location = location
            };

            DbContext.Packages.Add(newPackage);
            DbContext.SaveChanges();

            return newPackage;
        }
    }
}