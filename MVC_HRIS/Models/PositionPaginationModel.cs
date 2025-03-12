using API_HRIS.Models;

namespace API_HRIS.ApplicationModel
{
    public class PositionPaginateModel
    {
        public string? CurrentPage { get; set; }
        public string? NextPage { get; set; }
        public string? PrevPage { get; set; }
        public string? TotalPage { get; set; }
        public string? PageSize { get; set; }
        public string? TotalRecord { get; set; }
        public List<TblPositionModel> items { get; set; }


    }
}
