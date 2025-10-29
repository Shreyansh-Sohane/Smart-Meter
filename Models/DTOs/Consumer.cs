using System.ComponentModel.DataAnnotations;

namespace SmartMeterBackend.Models.DTOs
{
    
    public class AddConsumerRequest
    {
        

        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int Orgunitid { get; set; }

        public int Tariffid { get; set; }

        public string Status { get; set; } = null!;

    }
    public class UpdateConsumerRequest
    {
        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int Orgunitid { get; set; }

        public int Tariffid { get; set; }

        public string Status { get; set; } = null!;

        public bool Deleted { get ; set; }
    }


}
