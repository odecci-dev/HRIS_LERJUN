﻿using API_HRIS.Manager;
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
            if (employee == null)
                throw new InvalidOperationException("Employee not found");
            var existingRecord = await _context.TblPayslipModel.Where(a=>a.EmployeeId == data.EmployeeID.ToString()).ToListAsync();


            if (existingRecord.Any())
            {
                // Remove the record
                _context.TblPayslipModel.RemoveRange(existingRecord);
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            decimal renderedHours = dbmet.TimeLogsData().ToList()
                .Where(a => a.UserId == data.EmployeeID.ToString() && !excludedTaskIds.Contains(int.Parse(a.TaskId))
                    && Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom)
                    && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto) && a.StatusId == "1").ToList()
                .Sum(a => {
                    if (decimal.TryParse(a.RenderedHours, out decimal value))
                        return value;
                    return 0;
                });
            var totalSalary = renderedHours * decimal.Parse(employee.Rate);
            if (!decimal.TryParse(totalSalary.ToString(), out decimal grossSalary))
                throw new InvalidOperationException("Employee salary rate is invalid.");

            //_logger.LogInformation($"Computing Payslip for Employee ID: {data.EmployeeID}");

            decimal sssDeduction = await dbmet.ComputeSSSContribution(grossSalary);
            decimal philHealthDeduction = await dbmet.ComputePhilHealthDeduction(grossSalary);
            decimal pagIbigDeduction = await dbmet.ComputePagIbigDeduction(grossSalary);
            decimal taxDeduction = await dbmet.ComputeWithholdingTax(grossSalary, employee.PayrollType);
            decimal totalDeduction = (sssDeduction + philHealthDeduction + pagIbigDeduction);
            decimal taxableIncome = grossSalary - totalDeduction;
            decimal netSalary = taxableIncome - taxDeduction;

            var payslip = new TblPayslipModel
            {
                RenderedHours = renderedHours,
                EmployeeId = data.EmployeeID.ToString(),
                GrossSalary = grossSalary,
                SSSDeduction = sssDeduction,
                PhilHealthDeduction = philHealthDeduction,
                PagIbigDeduction = pagIbigDeduction,
                TaxDeduction = taxDeduction,
                NetSalary = netSalary,
                DateCreated = DateTime.Now
            };

            _context.TblPayslipModel.Add(payslip);
            await _context.SaveChangesAsync();

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
                               TotalDeductions = py.TaxDeduction + py.SSSDeduction + py.PhilHealthDeduction + py.PagIbigDeduction
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

            var emplist =  _context.TblUsersModels.ToList();
            var paylist = _context.TblPayslipModel.ToList();
            if(paylist.Count != 0)
            {
                _context.TblPayslipModel.RemoveRange(paylist);
            }
           
            await _context.SaveChangesAsync(); // Save changes to the database
            for (int x = 0; x < emplist.Count; x++)
            {
                var records = _context.TblTimeLogs
                      .Where(r => r.Date >= Convert.ToDateTime(data.datefrom) && r.Date <= Convert.ToDateTime(data.dateto)
                      && r.UserId == emplist[x].Id )
                      .Select(r => r.Date) // Remove time component
                      .Distinct()
                      .ToList();

                // Step 2: Generate all dates within the range
                var allDates = GetAllDatesInRange(Convert.ToDateTime(data.datefrom), Convert.ToDateTime(data.dateto));

                // Step 3: Find missing dates
                var missingDates = allDates.Except(allDates).ToList();

                // Step 4: Build the response
                var response = new
                {
                    TotalUniqueRecords = records.Count,
                    ExpectedTotalDates = allDates.Count,
                    MissingDates = missingDates
                };

                var excludedTaskIds = new List<int> { 10, 11, 12 };
                var empid = emplist[x].EmployeeId;
            
                var existingRecord = await _context.TblPayslipModel.Where(a => a.EmployeeId == emplist[x].EmployeeId.ToString()).FirstOrDefaultAsync();

          
                decimal renderedHours = dbmet.TimeLogsData()
                                      .ToList()
                                      .Where(a => !excludedTaskIds.Contains(int.Parse(a.TaskId))
                                          && a.UserId == emplist[x].Id.ToString()
                                          && Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom)
                                          && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)
                                          && a.StatusId == "1")
                                      .Sum(a => decimal.TryParse(a.RenderedHours ?? "0", out decimal value) ? value : 0);
                
                decimal rates = emplist[x].Rate == null ? 0 : decimal.Parse(emplist[x].Rate);
                
                var totalSalary = renderedHours * rates;
                
                if (!decimal.TryParse(totalSalary.ToString(), out decimal grossSalary))
                    throw new InvalidOperationException("Employee salary rate is invalid.");

       
                decimal sssDeduction = await dbmet.ComputeSSSContribution(grossSalary);
                decimal philHealthDeduction = await dbmet.ComputePhilHealthDeduction(grossSalary);
                decimal pagIbigDeduction = await dbmet.ComputePagIbigDeduction(grossSalary);
                decimal taxDeduction = await dbmet.ComputeWithholdingTax(grossSalary, emplist[x].PayrollType);
                decimal totalDeduction = (sssDeduction + philHealthDeduction + pagIbigDeduction);
                decimal taxableIncome = grossSalary - totalDeduction;
                decimal netSalary = taxableIncome - taxDeduction;

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
                    DateCreated = DateTime.Now
                };

                _context.TblPayslipModel.Add(payslip);
                await _context.SaveChangesAsync();



            }
            try
            {

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
                                            TotalDeductions = py.TaxDeduction + py.SSSDeduction + py.PhilHealthDeduction + py.PagIbigDeduction
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
