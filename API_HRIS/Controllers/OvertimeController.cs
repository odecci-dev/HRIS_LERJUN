using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class OvertimeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public OvertimeController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            this.dbmet = _dbmet; 
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
                             where ot.isDeleted == false && ot.EmployeeNo == data.EmployeeNo
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
                //var result = (dynamic)null;
                //result = _context.TblOvertimeModel.Where(a => a.isDeleted == false && a.EmployeeNo == data.EmployeeNo).OrderByDescending(a => a.Id).ToList();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> PendingOvertTimeList(EmployeeIdFilter data)
        {
            var result = (dynamic)null;
            result = _context.TblOvertimeModel.Where(a => a.isDeleted == false && a.EmployeeNo == data.EmployeeNo).OrderByDescending(a => a.Id).ToList();
            if (data.EmployeeNo == "0")
            {
                result = from ot in _context.TblOvertimeModel
                         join user in _context.TblUsersModels
                         on ot.EmployeeNo equals user.EmployeeId
                         join leave in _context.TblLeaveTypeModel
                         on ot.LeaveId equals leave.Id into leavegroup
                         from leave in leavegroup.DefaultIfEmpty()
                         join status in _context.TblStatusModels
                         on ot.Status equals status.Id into statusgroup
                         from status in statusgroup.DefaultIfEmpty()
                         where ot.isDeleted == false && ot.Status == 1004
                         select new
                         {
                             ot.Id,
                             ot.OTNo,
                             ot.EmployeeNo,
                             Date = ot.Date != null ? ot.Date.Value.ToString("yyyy-MM-dd") : null,
                             user.Fullname,
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
            }
            else
            {
                result = from ot in _context.TblOvertimeModel
                         join user in _context.TblUsersModels
                         on ot.EmployeeNo equals user.EmployeeId
                         join leave in _context.TblLeaveTypeModel
                         on ot.LeaveId equals leave.Id into leavegroup
                         from leave in leavegroup.DefaultIfEmpty()
                         join status in _context.TblStatusModels
                         on ot.Status equals status.Id into statusgroup
                         from status in statusgroup.DefaultIfEmpty()
                         where ot.isDeleted == false && ot.EmployeeNo == data.EmployeeNo && ot.Status == 1004
                         select new
                         {
                             ot.Id,
                             ot.OTNo,
                             ot.EmployeeNo,
                             Date = ot.Date != null ? ot.Date.Value.ToString("yyyy-MM-dd") : null,
                             user.Fullname,
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
            }




            return Ok(result);
        }
        public partial class TblOvertimeModelView
        {
            public int Id { get; set; }
            public string? OTNo { get; set; }
            public string? EmployeeNo { get; set; }
            public string? Date { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? HoursFiled { get; set; }
            public string? HoursApproved { get; set; }
            public string? Remarks { get; set; }
            public string? ConvertToLeave { get; set; }
            public string? DateCreated { get; set; }
            public int? LeaveId { get; set; }
            public int? Status { get; set; }
            public int? CreatedBy { get; set; }
            public string? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public string? DateDeleted { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblOvertimeModel>> save(TblOvertimeModelView data)
        {
            if (_context.TblOvertimeModel == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }


            if (data.CreatedBy == null)
            {

                string query = $@"UPDATE TblOvertimeModel
	                            SET isDeleted = 1"
                            + " WHERE Id = '" + data.Id + "'";
                db.AUIDB_WithParam(query);

                string status = "Overtime request has been successfully Deleted";
                return Ok(status);
            }

            try
            {
                string status = "";
                //_context.TblPositionModels.Add(tblPositionModel);
                if (data.Id == 0)
                {
                    string query = $@"INSERT INTO [TblOvertimeModel] 
                     ([EmployeeNo],[Date],[StartTime],[EndTime],[StartDate],[EndDate],[HoursFiled],[Remarks],[ConvertToLeave],[DateCreated],[isDeleted],[CreatedBy],[Status])
                     VALUES ('" + data.EmployeeNo + "','" + data.Date + "','" + data.StartTime + "','" + data.EndTime + "','" + data.StartDate + "','" + data.EndDate + "','" + data.HoursFiled + "','" + data.Remarks + "','" + data.ConvertToLeave + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','0','" + data.CreatedBy + "','1004');";
                    
                    db.AUIDB_WithParam(query);
                    //_context.TblOvertimeModel.Add(data);

                    //await _context.SaveChangesAsync();
                    status = "Overtime request has been successfully saved";
                    dbmet.InsertAuditTrail("Save Overtime request" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Overtime Module", "User", "0");
                    //_context.SaveChanges();

                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Overtime request" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class OTUpdateStatus
        {
            public int Id { get; set; }
            public int status { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblOvertimeModel>> updateStatus(OTUpdateStatus data)
        {


            try
            {
                string status = "";
                //_context.TblPositionModels.Add(tblPositionModel);
                string query = $@"UPDATE TblOvertimeModel
	                            SET Status = " + data.status
                           + " WHERE Id = '" + data.Id + "'";
                db.AUIDB_WithParam(query);

                status = "Overtime request has been successfully Updated";
                return Ok(status);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Overtime request" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }


    }
}
