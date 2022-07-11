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
    public class WarehousesController : ControllerBase
    {
        public WarehouseRepository WarehouseRepository { get; }

        public WarehousesController(WarehouseRepository warehouseRepository)
        {
            WarehouseRepository = warehouseRepository;
        }

        [HttpGet]
        public IActionResult GetWarehousesList()
        {
            return Ok(WarehouseRepository.GetAllWarehouses());
        }

        [HttpGet("{id}")]
        public IActionResult GetWarehouse(int id)
        {
            return Ok(WarehouseRepository.GetWarehouseById(id));
        }

        [HttpGet("{id}/locations")]
        public IActionResult GetWarehouseLocations([FromRoute] int id, [FromQuery] LocationStateFilter locationStateFilter = LocationStateFilter.Free)
        {
            List<Location> locations = locationStateFilter
            switch
            {
            LocationStateFilter.Free => WarehouseRepository.GetWarehouseFreeLocations(id),
            LocationStateFilter.Assigned => WarehouseRepository.GetWarehouseOccupiedLocations(id),
            _ => WarehouseRepository.GetWarehouseLocations(id),
            };

            return Ok(locations);
        }
    }
}