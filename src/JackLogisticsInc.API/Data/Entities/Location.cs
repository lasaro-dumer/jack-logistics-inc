namespace JackLogisticsInc.API.Data.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Corridor { get; set; }
        public string Shelf { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }
    }
}