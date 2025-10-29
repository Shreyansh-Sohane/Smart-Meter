namespace SmartMeterBackend.Models.DTOs
{

    public class AddOrgUnitRequest
    {
        public string Type { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int? Parentid { get; set; }
    }
    public class UpdateOrgUnitRequest
    {
        public string Type { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int? Parentid { get; set; }
    }

    public class OrgUnitResponse
    {
        public int Orgunitid { get; set; }
    
        public string Type { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int? Parentid { get; set; }
    }
}
