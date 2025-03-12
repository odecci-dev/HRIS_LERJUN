using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Fullname { get; set; }

        public string? Fname { get; set; }

        public string? Lname { get; set; }
        public string? Mname { get; set; }
        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Gender { get; set; }

        public string? EmployeeID { get; set; }

        public int? CorporateID { get; set; }

        public int? PositionID { get; set; }
        //
        public string? JWToken { get; set; }

        public string? FilePath { get; set; }

        public string? OTP { get; set; }

        public string? Cno { get; set; }
        public int? Type { get; set; }

        public int? Active { get; set; }
        public int? isVIP { get; set; }
        public int? AllowEmailNotif { get; set; }


    }
}
