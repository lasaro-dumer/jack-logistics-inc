using System;
using System.Collections.Generic;
using System.Linq;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JackLogisticsInc.API.Data.Repositories
{
    public class ShipmentsRepository
    {
        public LogisticsDbContext DbContext { get; }

        public ShipmentsRepository(LogisticsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public List<Shipment> GetAllShipments()
        {
            return DbContext.Shipments.Include(s => s.Packages).ToList();
        }

        public int GetInTransitShipmentsCount()
        {
            return DbContext.Shipments.Where(s => !s.DeliveredAt.HasValue).Count();
        }

        public Shipment SavePackageShipmentAndDepart(string destinationAddressData, Package package, DateTime eta)
        {
            Shipment shipment = new Shipment()
            {
                DestinationAddress = destinationAddressData,
                LeftForDestinationAt = DateTime.UtcNow,
                EstimatedTimeOfArrival = eta
            };
            shipment.Packages.Add(package);

            //departing, leaving the location free
            package.Location = null;

            DbContext.Shipments.Add(shipment);
            DbContext.SaveChanges();

            return shipment;
        }
    }
}