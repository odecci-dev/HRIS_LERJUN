using API_HRIS.Models;

namespace MVC_HRIS.Models
{
    public class PaginationUserModel
    {
        public string? CurrentPage { get; set; }
        public string? NextPage { get; set; }
        public string? PrevPage { get; set; }
        public string? TotalPage { get; set; }
        public string? PageSize { get; set; }
        public string? TotalRecord { get; set; }
        public string? TotalVIP { get; set; }
        public List<GetAllUserDetailsResult> items { get; set; }
    }
}
