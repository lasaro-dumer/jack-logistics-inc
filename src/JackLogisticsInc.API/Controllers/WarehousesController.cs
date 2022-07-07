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
    public class WarehousesController : ControllerBase
    {
        public LogisticsDbContext DbContext { get; set; }

        public WarehousesController(LogisticsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetWarehousesList()
        {
            return Ok(this.DbContext.Warehouses.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetWarehouse(int id)
        {
            return Ok(this.DbContext.Warehouses.First(w => w.Id == id));
        }

        [HttpGet("{id}/locations")]
        public IActionResult GetWarehouseLocations([FromRoute] int id, [FromQuery] LocationStateFilter locationStateFilter = LocationStateFilter.Free)
        {
            IQueryable<Location> query = DbContext.Locations
                .Include(l => l.Package)
                .Where(l => l.WarehouseId == id);

            switch (locationStateFilter)
            {
                case LocationStateFilter.Free:
                    query = query.Where(l => l.Package == null);
                    break;
                case LocationStateFilter.Assigned:
                    query = query.Where(l => l.Package != null);
                    break;
                case LocationStateFilter.Any:
                default:
                    break;
            }

            return Ok(query.ToList());
        }
    }
}