using System.Collections.Generic;
using System.Linq;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Data.Repositories;
using JackLogisticsInc.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JackLogisticsInc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController : ControllerBase
    {
        public PackagesRepository PackagesRepository { get; }
        public WarehouseRepository WarehouseRepository { get; }

        public PackagesController(PackagesRepository packagesRepository, WarehouseRepository warehouseRepository)
        {
            PackagesRepository = packagesRepository;
            WarehouseRepository = warehouseRepository;
        }

        [HttpGet]
        public IActionResult GetPackages()
        {
            return base.Ok(PackagesRepository.GetPackagesList());
        }

        [HttpGet("{id}")]
        public IActionResult GetPackageById(int id)
        {
            Package package = PackagesRepository.GetPackageById(id);

            if (package == null)
                return BadRequest("Invalid package ID");

            return Ok(package);
        }

        [HttpPost]
        public IActionResult AddPackage(AddPackageModel addPackageModel)
        {
            Location location = WarehouseRepository.GetLocationById(addPackageModel.LocationId);

            if (location == null)
                //Not a fan of 400 when an item is not found, would prefer a 404, but most client apps/developers react better to 400
                return BadRequest($"Location {addPackageModel.LocationId} not found");
            if (location.Package != null)
                return BadRequest($"Location {addPackageModel.LocationId} is already occupied");

            Package newPackage = PackagesRepository.AddPackage(addPackageModel.Description, location);

            return Created($"/api/packages/{newPackage.Id}", newPackage);
        }
    }
}