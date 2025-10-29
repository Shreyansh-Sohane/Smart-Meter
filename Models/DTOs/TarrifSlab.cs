namespace SmartMeterBackend.Models.DTOs
{
    public class AddTariffslabRequest
    {
        public int Tariffid { get; set; }
        public decimal Fromkwh { get; set; }
        public decimal Tokwh { get; set; }
        public decimal Rateperkwh { get; set; }
        public bool Deleted { get; set; } = false;
    }

    public class UpdateTariffslabRequest
    {
        public int Tariffslabid { get; set; }
        public int Tariffid { get; set; }
        public decimal Fromkwh { get; set; }
        public decimal Tokwh { get; set; }
        public decimal Rateperkwh { get; set; }
        public bool Deleted { get; set; }
    }

    public class TariffslabResponse
    {
        public int Tariffslabid { get; set; }
        public int Tariffid { get; set; }
        public decimal Fromkwh { get; set; }
        public decimal Tokwh { get; set; }
        public decimal Rateperkwh { get; set; }
        public bool Deleted { get; set; }
    }
}
