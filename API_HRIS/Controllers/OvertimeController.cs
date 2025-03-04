using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class OvertimeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        DBMethods dbmet = new DBMethods();

        public OvertimeController(ODC_HRISContext context)
        {
            _context = context;
        }
        public class EmployeeIdFilter
        {
            public string EmployeeNo { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> OvertTimeList(EmployeeIdFilter data)
        {
            try
            {
                var result = from ot in _context.TblOvertimeModel
                             join leave in _context.TblLeaveTypeModel
                             on ot.LeaveId equals leave.Id into leavegroup
                             from leave in leavegroup.DefaultIfEmpty()
                             join status in _context.TblStatusModels
                             on ot.Status equals status.Id into statusgroup
                             from status in statusgroup.DefaultIfEmpty()
                             where ot.isDeleted == false  && ot.EmployeeNo == data.EmployeeNo
                             select new
                             {
                                 ot.Id,
                                 ot.OTNo,
                                 ot.EmployeeNo,
                                 Date = ot.Date != null ? ot.Date.Value.ToString("yyyy-MM-dd") : null,

                                 ot.StartTime,
                                 ot.EndTime,
                                 StartDate = ot.StartDate != null ? ot.StartDate.Value.ToString("yyyy-MM-dd") : null,
                                 EndDate = ot.EndDate != null ? ot.EndDate.Value.ToString("yyyy-MM-dd") : null,
                                 ot.HoursFiled,
                                 ot.HoursApproved,
                                 ot.Remarks,
                                 ot.ConvertToLeave,
                                 leave.Name,
                                 LeaveName = leave != null ? leave.Name : "No Leave", // Handle NULL values
                                 LeaveRemarks = leave != null ? leave.Remarks : "",
                                 StatusName = status != null ? status.Status : "Unknown", // Handle NULL values
                                 ot.Status
                             };

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
        }
   

    }
}
