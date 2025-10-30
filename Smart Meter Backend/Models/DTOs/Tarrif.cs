namespace SmartMeterBackend.Models.DTOs
{
    public class AddTariffRequest
    {
        public string Name { get; set; } = null!;
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
    }
    public class UpdateTariffRequest
    {
        public string Name { get; set; } = null!;
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
    }

    public class TariffResponse
    {
        public int TariffId { get; set; }
        public string Name { get; set; } = null!;
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
    }
}
