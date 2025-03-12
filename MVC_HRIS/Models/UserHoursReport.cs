namespace MVC_HRIS.Models;
public class UserHoursReport
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public decimal ApprovedHours { get; set; }
    public decimal BreakHours { get; set; }
    public decimal PendingHours { get; set; }
    public decimal TotalRenderedHours { get; set; }
}
