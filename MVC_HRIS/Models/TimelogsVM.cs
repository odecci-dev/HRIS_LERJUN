namespace MVC_HRIS.Models;

public class TimelogsVM
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? Date { get; set; }
    public string? TimeIn { get; set; }
    public string? TimeOut { get; set; }
    public string? RenderedHours { get; set; }
    public string? LunchIn { get; set; }
    public string? LunchOut { get; set; }
    public decimal? TotalLunchHours { get; set; }
    public string? BreakInAm { get; set; }
    public string? BreakOutAm { get; set; }
    public decimal TotalBreakAmHours { get; set; }
    public string? BreakInPm { get; set; }
    public string? BreakOutPm { get; set; }
    public decimal? TotalBreakPmHours { get; set; }
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
    public string? DepartmentName { get; set; }
    public string? Position { get; set; }
    public string? PositionLevel { get; set; }
    public string? EmployeeType { get; set; }
}

