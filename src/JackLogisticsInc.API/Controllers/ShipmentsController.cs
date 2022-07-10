using System;
using System.Linq;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace src.JackLogisticsInc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        public LogisticsDbContext DbContext { get; set; }

        public ShipmentsController(LogisticsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetShipments()
        {
            return Ok(DbContext.Shipments.ToList());
        }

        [HttpPost]
        public IActionResult ShipPackage(ShipPackageModel shipPackageModel)
        {
            if (string.IsNullOrEmpty(shipPackageModel.DestinationAddressData))
                return BadRequest($"Package {shipPackageModel.PackageId} needs a destination address to be shipped");

            Package package = DbContext.Packages.Include(p => p.Location)
                .FirstOrDefault(p => p.Id == shipPackageModel.PackageId);

            if (package == null)
                //Not a fan of 400 when an item is not found, would prefer a 404, but most client apps/developers react better to 400
                return BadRequest($"Package {shipPackageModel.PackageId} not found");
            if (package.Shipment != null)
                return BadRequest($"Package {shipPackageModel.PackageId} is already shipped");

            int inTransitShipments = DbContext.Shipments.Where(s => !s.DeliveredAt.HasValue).Count();
            //Only one truck available for shipment, so can't have more shipments in transit. Should be expanded to have a whole truck management logic to support an N number of trucks
            if (inTransitShipments > 0)
                return BadRequest($"No truck available for shipment");

            //TODO: Implement a real ETA calculation (using GoogleMaps?)
            DateTime eta = DateTime.UtcNow.AddMinutes(3);

            Shipment shipment = new Shipment()
            {
                DestinationAddress = shipPackageModel.DestinationAddressData,
                LeftForDestinationAt = DateTime.UtcNow,
                EstimatedTimeOfArrival = eta
            };
            shipment.Packages.Add(package);

            package.Location = null;

            DbContext.Shipments.Add(shipment);
            DbContext.SaveChanges();

            //TODO: add background job to simulate truck travel

            return Created($"/api/shipments/{shipment.Id}", shipment);
        }
    }
}