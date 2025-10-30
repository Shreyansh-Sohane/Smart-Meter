namespace SharedLibrary.Models
{
    public class MeterReading
    {
        public string meterid { get; set; }
        public string meterreadingdate { get; set; }
        public double energyconsumed { get; set; }
        public double voltage { get; set; }
        public double current { get; set; }
    }
}