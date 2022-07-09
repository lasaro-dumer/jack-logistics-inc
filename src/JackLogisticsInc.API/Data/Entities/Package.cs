namespace JackLogisticsInc.API.Data.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
    }
}