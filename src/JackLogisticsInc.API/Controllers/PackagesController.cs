using System.Linq;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using JackLogisticsInc.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JackLogisticsInc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController : ControllerBase
    {
        public LogisticsDbContext DbContext { get; set; }

        public PackagesController(LogisticsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetPackages()
        {
            return Ok(DbContext.Packages
                .Include(p => p.Location)
                .Include(p => p.Shipment)
                .ToList());
        }

        [HttpPost]
        public IActionResult AddPackage(AddPackageModel addPackageModel)
        {
            Location location = DbContext.Locations
                .Include(l => l.Package)
                .FirstOrDefault(l => l.Id == addPackageModel.LocationId);

            if (location == null)
                //Not a fan of 400 when an item is not found, would prefer a 404, but most client apps/developers react better to 400
                return BadRequest($"Location {addPackageModel.LocationId} not found");
            if (location.Package != null)
                return BadRequest($"Location {addPackageModel.LocationId} is already occupied");

            Package newPackage = new Package()
            {
                Description = addPackageModel.Description,
                Location = location
            };

            DbContext.Packages.Add(newPackage);
            DbContext.SaveChanges();

            return Created($"/api/packages/{newPackage.Id}", newPackage);
        }
    }
}