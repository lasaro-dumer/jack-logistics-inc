using System;
using System.Collections.Generic;

namespace JackLogisticsInc.API.Data.Entities
{
    public class Shipment
    {
        public int Id { get; set; }
        public List<Package> Packages { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime LeftForDestinationAt { get; set; }
        public DateTime EstimatedTimeOfArrival { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public Shipment()
        {
            Packages = new List<Package>();
        }
    }
}