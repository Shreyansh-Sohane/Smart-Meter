using System;

namespace SmartMeterBackend.Models.DTOs
{
    public class TodruleResponse
    {
        public int TodruleId { get; set; }
        public int TariffId { get; set; }
        public string Name { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal RatePerKwh { get; set; }
        public bool Deleted { get; set; }
    }

    public class AddTodruleRequest
    {
        public int TariffId { get; set; }
        public string Name { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal RatePerKwh { get; set; }
        public bool Deleted { get; set; }
    }

    public class UpdateTodruleRequest
    {
        public int TodruleId { get; set; }
        public int TariffId { get; set; }
        public string Name { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal RatePerKwh { get; set; }
        public bool Deleted { get; set; }
    }
}
