using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_HRIS.Models
{
    public class TblModulesModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Img { get; set; }
        public string? Link { get; set; }
        public string? Class { get; set; }
        public int? Status { get; set; }
    }
}
