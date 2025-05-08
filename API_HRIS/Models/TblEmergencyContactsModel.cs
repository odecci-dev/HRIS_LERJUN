namespace API_HRIS.Models
{
    public class TblEmergencyContactsModel
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }
        public string? Name { get; set; }

        public string? Relationship { get; set; }

        public string? PhoneNumber { get; set; }

    }
}
