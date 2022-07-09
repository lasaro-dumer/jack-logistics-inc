using System.Collections.Generic;

namespace JackLogisticsInc.API.Data.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddressData { get; set; }
        public List<Location> Locations { get; set; }

        public Warehouse()
        {
            Locations = new List<Location>();
        }
    }
}