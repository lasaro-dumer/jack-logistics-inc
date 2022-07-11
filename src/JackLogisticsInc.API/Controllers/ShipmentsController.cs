using System;
using System.Linq;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Data.Repositories;
using JackLogisticsInc.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace src.JackLogisticsInc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        public PackagesRepository PackagesRepository { get; }
        public ShipmentsRepository ShipmentsRepository { get; }

        public ShipmentsController(PackagesRepository packagesRepository, ShipmentsRepository shipmentsRepository)
        {
            PackagesRepository = packagesRepository;
            ShipmentsRepository = shipmentsRepository;
        }

        [HttpGet]
        public IActionResult GetShipments()
        {
            return Ok(ShipmentsRepository.GetAllShipments());
        }

        [HttpPost]
        public IActionResult ShipPackage(ShipPackageModel shipPackageModel)
        {
            if (string.IsNullOrEmpty(shipPackageModel.DestinationAddressData))
                return BadRequest($"Package {shipPackageModel.PackageId} needs a destination address to be shipped");

            Package package = PackagesRepository.GetPackageById(shipPackageModel.PackageId);

            if (package == null)
                //Not a fan of 400 when an item is not found, would prefer a 404, but most client apps/developers react better to 400
                return BadRequest($"Package {shipPackageModel.PackageId} not found");
            if (package.Shipment != null)
                return BadRequest($"Package {shipPackageModel.PackageId} is already shipped");

            int inTransitShipments = ShipmentsRepository.GetInTransitShipmentsCount();
            //Only one truck available for shipment, so can't have more shipments in transit. Should be expanded to have a whole truck management logic to support an N number of trucks
            if (inTransitShipments > 0)
                return BadRequest($"No truck available for shipment");

            //TODO: Implement a real ETA calculation (using GoogleMaps?)
            DateTime eta = DateTime.UtcNow.AddMinutes(1);

            Shipment shipment = ShipmentsRepository.SavePackageShipmentAndDepart(shipPackageModel.DestinationAddressData, package, eta);

            return Created($"/api/shipments/{shipment.Id}", shipment);
        }
    }
}