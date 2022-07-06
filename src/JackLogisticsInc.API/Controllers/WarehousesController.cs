using System.Linq;
using JackLogisticsInc.API.Data;
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
        public IActionResult GetWarehouseLocations(int id)
        {
            return Ok(this.DbContext.Locations.Where(l => l.WarehouseId == id).ToList());
        }
    }
}