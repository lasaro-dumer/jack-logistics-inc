namespace JackLogisticsInc.API.Data.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string GeolocationData { get; set; }
    }
}