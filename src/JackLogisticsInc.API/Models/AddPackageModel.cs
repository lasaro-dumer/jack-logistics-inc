using JackLogisticsInc.API.Data.Entities;

namespace JackLogisticsInc.API.Models
{
    public class AddPackageModel
    {
        public string Description { get; set; }
        public int LocationId { get; set; }
    }
}