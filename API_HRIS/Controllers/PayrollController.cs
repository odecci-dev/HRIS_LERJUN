using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Drawing.Printing;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using API_HRIS.ApplicationModel;
using System.Text;
using static API_HRIS.ApplicationModel.EntityModels;
using System.Runtime.Intrinsics.X86;
using System.Linq;
using API_HRIS.Migrations;
using static API_HRIS.Controllers.TicketingController;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class PayrollController : ControllerBase 
    {
        private readonly ODC_HRISContext _context;
        private readonly DBMethods dbmet;
        DbManager db = new DbManager();

        public PayrollController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet ?? throw new ArgumentNullException(nameof(_dbmet));
        }
        public class EmployeeFilters
        {
            public int? EmployeeID { get; set; }
            public string datefrom { get; set; }
            public string dateto { get; set; }
        }
      
        //[HttpPost]
        //public async Task<IActionResult> TESTTEST(IdFilter data)
        //{
        //    //var tax = ComputePayslip(data);
        //    return Ok("123");
        //}

       
        [HttpGet]
        public async Task<IActionResult> PayrollGenerate()
        {

            return Ok(_context.TblPayslipModel.Where(a => a.isDeleted == false).OrderByDescending(a => a.PayslipId).ToList());
        }
        [HttpPost]
        public async Task<IActionResult> PayrollGenerate2(IdFilter data)
        {

            return Ok(_context.TblPayslipModel.Where(a => a.isDeleted == false).OrderByDescending(a => a.PayslipId).ToList());
        }
      
      
        [HttpPost]
        public async Task<IActionResult> ComputePayslip(EmployeeFilters data)
        {
            var excludedTaskIds = new List<int> { 10, 11, 12 };
            var employee = await _context.TblUsersModels.FindAsync(data.EmployeeID);
           

            var result = (from user in _context.TblUsersModels
                          join dept in _context.TblDeparmentModels
                          on user.Department equals dept.Id into DeptGroup
                          from dept in DeptGroup.DefaultIfEmpty()
                          join pos in _context.TblPositionModels
                          on user.PositionLevelId equals pos.Id into PosGroup
                          from pos in PosGroup.DefaultIfEmpty()
                          select new { user, dept, pos })
                     .AsEnumerable() // Switch to client-side processing
                     .Join(_context.TblPayslipModel.AsEnumerable(),
                           u => u.user.Id,
                           py => int.Parse(py.EmployeeId), // int.Parse now works since it's processed in memory
                           (u, py) => new
                           {
                               Status=u.user.Status,
                               RenderedHours = py.RenderedHours,
                               EmployeeName = u.user.Fullname,
                               EmployeeNumber = u.user.EmployeeId,
                               JobTitle = u.pos?.Name ?? "N/A",
                               Department = u.dept?.DepartmentName ?? "N/A",
                               PayslipNumber = py.PayslipNumber,
                               TaxNumber = "",
                               PayDate = py.DateCreated,
                               GrossPay = py.GrossSalary,
                               NetPay = py.NetSalary,
                               Tax = py.TaxDeduction,
                               SSS = py.SSSDeduction,
                               PhilHealth = py.PhilHealthDeduction,
                               PagIbig = py.PagIbigDeduction,
                               OtherDeductions = "0.00",
                               TotalDeductions = py.TaxDeduction + py.SSSDeduction + py.PhilHealthDeduction + py.PagIbigDeduction,
                                    OvertimeHours = py.OTHours,
                               OvertimePay = py.OTPay,
                               DaysAbsent = py.DaysAbsent,
                               DaysPresent = py.DaysPresent,
                               AbsentDeduction = py.AbsentDeduction
                           })
                     .Where(x => x.EmployeeNumber == employee.EmployeeId.ToString())
                     .ToList();

            return Ok(result);
        }
        private List<DateTime> GetAllDatesInRange(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();
            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                dates.Add(date);
            }
            return dates;
        }
        [HttpPost]
        public async Task<IActionResult> ComputePayslipAdmin(EmployeeFilters data)
        {
            var result = (dynamic)null;
            decimal overtime = 0;
            int workdays = 0;
            decimal nightdiff = 0;
            decimal holiday = 0;
            var excludedPosId = new List<int> { 1,5,6 };

            var emplist =  _context.TblUsersModels.Where(a=>a.PositionLevelId != 1 && a.EmployeeType != null).ToList();
            var paylist = _context.TblPayslipModel.ToList();
            if(paylist.Count != 0)
            {
                _context.TblPayslipModel.RemoveRange(paylist);
            }
           
            await _context.SaveChangesAsync(); // Save changes to the database
            for (int x = 0; x < emplist.Count; x++)
            {
                // Step 1: Get holidays from DB
                var holidays = _context.TblHolidayModel
                    .Where(h => h.Date >= Convert.ToDateTime(data.datefrom) && 
                    h.Date <= Convert.ToDateTime(data.dateto))
                    .Select(h => h.Date)
                    .ToList(); // List<DateTime>

                var records = _context.TblTimeLogs
                               .Where(r => r.Date >= Convert.ToDateTime(data.datefrom) &&
                                           r.Date <= Convert.ToDateTime(data.dateto) &&
                                           r.UserId == emplist[x].Id)
                               .Select(r => r.Date.Value) // Remove time part
                               .Distinct()
                               .ToList();
                DateTime fromDate = Convert.ToDateTime(data.datefrom);
                DateTime toDate = Convert.ToDateTime(data.dateto);

                var allWorkingDays = Enumerable.Range(0, (toDate - fromDate).Days + 1)
                                      .Select(offset => fromDate.AddDays(offset))
                                      .Where(d =>
                                          d.DayOfWeek != DayOfWeek.Saturday &&
                                          d.DayOfWeek != DayOfWeek.Sunday &&
                                          !holidays.Contains(d)
                                      )
                                      .ToList();
                var timeLogDates = _context.TblTimeLogs
                                .Where(t => t.UserId == emplist[x].Id && t.Date >= fromDate && t.Date <= toDate)
                                .Select(t => t.Date.Value)
                                .Distinct()
                                .ToList(); // List<DateTime>
                var missingDates = allWorkingDays.Except(records).ToList();
                var presentOnHolidays = timeLogDates
                                        .Intersect(holidays)
                                        .ToList(); // List<DateTime>
                var response = new
                {
                    TotalUniqueRecords = records.Count,
                    ExpectedWorkingDays = allWorkingDays.Count,
                    MissingWorkingDates = allWorkingDays.Except(records).ToList(),
                    MissingCount = allWorkingDays.Except(records).Count()
                };
                var othours = _context.TblOvertimeModel
                    .Where(a => a.EmployeeNo == emplist[x].EmployeeId
                                && a.Date >= fromDate
                                && a.Date <= toDate)
                    .Sum(a => a.HoursFiled ?? 0); 

                var excludedTaskIds = new List<int> { 10, 11, 12 };
                var empid = emplist[x].EmployeeId;
            
                var existingRecord = await _context.TblPayslipModel.Where(a => a.EmployeeId == emplist[x].EmployeeId.ToString()).FirstOrDefaultAsync();
                var totalSalary = (dynamic)null;
              
                decimal renderedHours = dbmet.TimeLogsData()
                                      .ToList()
                                      .Where(a => !excludedTaskIds.Contains(int.Parse(a.TaskId))
                                          && a.UserId == emplist[x].Id.ToString()
                                          && Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom)
                                          && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)
                                          && a.StatusId == "1")
                                      .Sum(a => decimal.TryParse(a.RenderedHours ?? "0", out decimal value) ? value : 0);

                decimal rates = 0;
                //decimal rates = emplist[x].Rate == null ? 0 : decimal.Parse(emplist[x].Rate);
                if (emplist[x].SalaryType == 1)
                {
                    rates = emplist[x].Rate == null ? 0 : decimal.Parse(emplist[x].Rate);
                    totalSalary = renderedHours * rates;
           
                }
                else if (emplist[x].SalaryType == 2)
                {
                    rates = emplist[x].Rate == null ? 0 : decimal.Parse(emplist[x].Rate) / int.Parse(emplist[x].DaysInMonth);
                    totalSalary = rates;
                   
                }
                else if (emplist[x].SalaryType == 1003)
                {
                    rates = (emplist[x].Rate == null ? 0 : decimal.Parse(emplist[x].Rate) / int.Parse(emplist[x].DaysInMonth)) / 8;
                    totalSalary = decimal.Parse(emplist[x].Rate) / 2;

                }
                else
                {
                    rates = emplist[x].Rate == null ? 0 : decimal.Parse(emplist[x].Rate) / int.Parse(emplist[x].DaysInMonth);
                    totalSalary = emplist[x].Rate;
                }
                decimal absent_deduct = 0;
                int daysabenst = allWorkingDays.Except(records).ToList().Count(); 
                if (allWorkingDays.Except(records).ToList().Count() > 0 )
                {
                    absent_deduct = rates * daysabenst;
                }
                var total_otpay = rates * 1.25m * othours;
                if (!decimal.TryParse(totalSalary.ToString(), out decimal grossSalary))
                    throw new InvalidOperationException("Employee salary rate is invalid.");

       
                decimal sssDeduction = emplist[x].EmployeeType == 1 ? await dbmet.ComputeSSSContribution(grossSalary) : 0;
                decimal philHealthDeduction = emplist[x].EmployeeType == 1 ?  await dbmet.ComputePhilHealthDeduction(grossSalary) : 0;
                decimal pagIbigDeduction = emplist[x].EmployeeType == 1 ?  await dbmet.ComputePagIbigDeduction(grossSalary) : 0;
                decimal taxDeduction =  await dbmet.ComputeWithholdingTax(grossSalary, emplist[x].PayrollType);
                decimal totalDeduction = (sssDeduction + philHealthDeduction + pagIbigDeduction + absent_deduct);
                decimal taxableIncome = grossSalary - totalDeduction;
                decimal netSalary = (taxableIncome - taxDeduction) + total_otpay;

                var payslip = new TblPayslipModel
                {
                    RenderedHours = renderedHours,
                    EmployeeId = emplist[x].Id.ToString(),
                    GrossSalary = grossSalary,
                    SSSDeduction = sssDeduction,
                    PhilHealthDeduction = philHealthDeduction,
                    PagIbigDeduction = pagIbigDeduction,
                    TaxDeduction = taxDeduction,
                    NetSalary = netSalary,
                    DateCreated = DateTime.Now,
                    OTHours = othours,
                    OTPay = total_otpay,
                    DaysAbsent = allWorkingDays.Except(records).ToList().Count(),
                    DaysPresent = records.Count,
                    AbsentDeduction = absent_deduct

                };

                _context.TblPayslipModel.Add(payslip);
                await _context.SaveChangesAsync();



            }
            try
            {
                var overtimeData = _context.TblOvertimeModel
                                    .GroupBy(a => a.EmployeeNo)
                                    .Select(g => new
                                    {
                                        EmployeeNo = g.Key,
                                        TotalOvertimeHours = g.Sum(a => a.HoursFiled) ?? 0
                                    })
                                    .ToDictionary(o => o.EmployeeNo, o => o.TotalOvertimeHours);

                // Fetch employee rates
                var employeeRates = emplist.ToDictionary(
    e => e.EmployeeId,
    e => e.Rate != null ? decimal.Parse(e.Rate.ToString()) : 0m // Handle nulls
);

                result = (from user in _context.TblUsersModels
                          join dept in _context.TblDeparmentModels
                          on user.Department equals dept.Id into DeptGroup
                          from dept in DeptGroup.DefaultIfEmpty()
                          join pos in _context.TblPositionModels
                          on user.Position equals pos.Id into PosGroup
                          from pos in PosGroup.DefaultIfEmpty()
                          select new { user, dept, pos })
                   .AsEnumerable() // Switch to client-side processing
                   .Join(_context.TblPayslipModel.AsEnumerable(),
                         u => u.user.Id,
                         py => int.Parse(py.EmployeeId), // int.Parse now works since it's processed in memory
                         (u, py) => new
                         {
                             RenderedHours = py.RenderedHours,
                             UserId = u.user.Id,
                             Status = u.user.Status,
                             EmployeeName = u.user.Fullname,
                             EmployeeNumber = u.user.EmployeeId,
                             JobTitle = u.pos?.Name ?? "N/A",
                             Department = u.dept?.DepartmentName ?? "N/A",
                             PayslipNumber = py.PayslipNumber,
                             TaxNumber = "",
                             PayDate = py.DateCreated,
                             GrossPay = py.GrossSalary,
                             NetPay = py.NetSalary,
                             Tax = py.TaxDeduction,
                             SSS = py.SSSDeduction,
                             PhilHealth = py.PhilHealthDeduction,
                             PagIbig = py.PagIbigDeduction,
                             OtherDeductions = "0.00",
                             TotalDeductions = py.TaxDeduction + py.SSSDeduction + py.PhilHealthDeduction + py.PagIbigDeduction,
                             OvertimeHours = py.OTHours,
                             OvertimePay = py.OTPay
            })
                   .ToList();

            
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
       
    }
}
