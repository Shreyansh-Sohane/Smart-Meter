namespace SmartMeterBackend.Models.DTOs
{
    public class ChangePasswordRequest
    {
        public string mobileNumber { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }

    }

    

}
