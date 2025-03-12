using API_HRIS.Models;

namespace API_HRIS.ApplicationModel
{
    public class EntityModels
    {
        public class CommonSearchFilterModel
        {
            public string? searchParam { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }
        public class FilterDepartment
        {
            public string? Department { get; set; }
            public int page { get; set; }
        }
        public class FilterEmployee
        {
            public string? Username { get; set; }
            public string? Department { get; set; }
            public int page { get; set; }
        }
        public class FilterPayrollType
        {
            public string? PayrollType { get; set; }
            public int page { get; set; }
        }
        public class FilterPosition
        {

            public string? Position { get; set; }
            public int page { get; set; }
        }
        public class FilterSalaryType
        {
            public string? SalaryType { get; set; }
            public int page { get; set; }
        }
        public class FilterUserType
        {
            public string? UserType { get; set; }
            public int page { get; set; }
        }
        public class PayrollTypePaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<TblPayrollType> items { get; set; }
        }
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
        public class RestorationModel
        {
            public int id { get; set; }
            public string? restoredBy { get; set; }
        }
        public class SalaryTypePaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<TblSalaryType> items { get; set; }
        }
        public class TimelogsVM
        {
            public string? Id { get; set; }
            public string? UserId { get; set; }
            public string? Date { get; set; }
            public string? TimeIn { get; set; }
            public string? TimeOut { get; set; }
            public string? RenderedHours { get; set; }
            public string? Username { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Email { get; set; }
            public string? EmployeeID { get; set; }
            public string? JWToken { get; set; }
            public string? FilePath { get; set; }
            public string? UsertypeId { get; set; }
            public string? UserType { get; set; }
            public string? DeleteFlagName { get; set; }
            public string? DeleteFlag { get; set; }
            public string? StatusName { get; set; }
            public string? StatusId { get; set; }
            public string? Remarks { get; set; }
            public string? TaskId { get; set; }
            public string? Task { get; set; }
            public string? Department { get; set; }
        }
        public class UserTypePaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<TblUserType> items { get; set; }
        }
        public class EmployeePaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<GetAllUserDetailsResult> items { get; set; }


        }
        public class DepartmentPaginationModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<TblDeparmentModel> items { get; set; }


        }
        public class DeletionModel
        {
            public int id { get; set; }
            public string deletedBy { get; set; }
        }
    }
}
