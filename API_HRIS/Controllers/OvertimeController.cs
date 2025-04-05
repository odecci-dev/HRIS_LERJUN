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
using System.Data;
using Microsoft.AspNetCore.Components.Forms;

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
            public string? EmployeeNo { get; set; }
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
                                 ot.ConvertToOffset,
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
        public class PedingOTFilter
        {
            public string? EmployeeNo { get; set; }
            public string? startDate { get; set; }
            public string? endDate { get; set; }
            public int? status { get; set; }
            public int? ManagerId { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> PendingOvertTimeList(PedingOTFilter data)
        {
            if (!DateTime.TryParse(data.startDate, out DateTime startDate))
            {
                return BadRequest("Invalid start date format.");
            }
            if (!DateTime.TryParse(data.endDate, out DateTime endDate))
            {
                return BadRequest("Invalid end date format.");
            }
            DateTime startD = DateTime.ParseExact(data.startDate, "yyyy-MM-dd", null);
            DateTime endD = DateTime.ParseExact(data.endDate, "yyyy-MM-dd", null);
            var result = from ot in _context.TblOvertimeModel

                    join user in _context.TblUsersModels
                    on ot.EmployeeNo equals user.EmployeeId

                    join department in _context.TblDeparmentModels
                    on user.Department equals department.Id into departmentgroup
                    from department in departmentgroup.DefaultIfEmpty()

                    join position in _context.TblPositionModels
                    on user.Position equals position.Id into positiongroup
                    from position in positiongroup.DefaultIfEmpty()
                     
                    join positionlvl in _context.TblPositionLevelModels
                    on user.PositionLevelId equals positionlvl.Id into positionlvlgroup
                    from positionlvl in positionlvlgroup.DefaultIfEmpty()
                     
                    join employeeType in _context.TblEmployeeTypes
                    on user.EmployeeType equals employeeType.Id into employeeTypegroup
                    from employeeType in employeeTypegroup.DefaultIfEmpty()

                    join leave in _context.TblLeaveTypeModel
                    on ot.LeaveId equals leave.Id into leavegroup
                    from leave in leavegroup.DefaultIfEmpty()

                    join status in _context.TblStatusModels
                    on ot.Status equals status.Id into statusgroup
                    from status in statusgroup.DefaultIfEmpty()

                    where ot.isDeleted == false && ot.Date >= startD && ot.Date <= endD

                    select new
                     {
                         ot.Id,
                         ot.OTNo,
                         ot.EmployeeNo,
                         Date = ot.Date != null ? ot.Date.Value.ToString("yyyy-MM-dd") : null,
                         user.Fullname,
                         Department = department.DepartmentName,
                         Position = position.Name,
                         PositionLevel = positionlvl.Level,
                         EmployeeType = employeeType.Title,
                         ManagerId = user.ManagerId,
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
                         Status= ot.Status
                     };
            
            if (data.status == 0)
            {
                result = result.Where(a => a.Status == 1004);
            }
            else
            {
                result = result.Where(a => a.Status == 1005);
            }
            if (data.EmployeeNo != "0")
            {
                result = result.Where(a => a.EmployeeNo == data.EmployeeNo);
            }
            if (data.ManagerId != 0)
            {
                result = result.Where(a => a.ManagerId == data.ManagerId);
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
            public string? ConvertToOffset { get; set; }
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
                     ([EmployeeNo],[Date],[StartTime],[EndTime],[StartDate],[EndDate],[HoursFiled],[Remarks],[ConvertToLeave],[ConvertToOffset],[DateCreated],[isDeleted],[CreatedBy],[Status])
                     VALUES ('" + data.EmployeeNo + "','" + data.Date + "','" + data.StartTime + "','" + data.EndTime + "','" + data.StartDate + "','" + data.EndDate + "','" + data.HoursFiled + "','" + data.Remarks + "','" + data.ConvertToLeave + "','" + data.ConvertToOffset + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','0','" + data.CreatedBy + "','1004');";
                    
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

        public class multiOTApprovalParamList
        {
            public int? Id { get; set; }
            public string? reason { get; set; }
            public decimal? HoursApproved { get; set; }
        }
        public class multiOTApprovalParam
        {
            public int? Status { get; set; }
            public List<multiOTApprovalParamList> otapproval { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblLeaveRequestModel>> MultiOTUpdateStatus(multiOTApprovalParam data)
        {
            string status = "";

            try
            {
                for (int i = 0; i < data.otapproval.Count; i++)
                {
                    Console.WriteLine(data.otapproval[i].Id);
                    var existingItem = await _context.TblOvertimeModel.FindAsync(data.otapproval[i].Id);
                    if (existingItem != null)
                    {
                        if (data.Status == 0)
                        {
                            existingItem.Status = 1005;
                            string query = $@"UPDATE TblOvertimeModel
	                            SET Status = '1005'"
                                + ", ApprovalReason  = '" + data.otapproval[i].reason + "'"
                                + ", HoursApproved = '0'"
                            + " WHERE Id = '" + data.otapproval[i].Id + "'";
                            db.AUIDB_WithParam(query);
                        }
                        else if (data.Status == 1)
                        {
                            existingItem.Status = 5;

                            string query = $@"UPDATE TblOvertimeModel
	                            SET Status = '5'"
                                + ", ApprovalReason  = '" + data.otapproval[i].reason + "'"
                                + ", HoursApproved = '" + data.otapproval[i].HoursApproved + "'"
                            + " WHERE Id = '" + data.otapproval[i].Id + "'";
                            db.AUIDB_WithParam(query);
                        }
                        //existingItem.ApprovalReason = data.otapproval[i].reason;
                        //existingItem.HoursApproved = data.otapproval[i].HoursApproved;


                        //_context.TblOvertimeModel.Update(existingItem);
                        //await _context.SaveChangesAsync();


                        status = "Overtime request successfully deleted";
                        dbmet.InsertAuditTrail("Update Overtime request" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Leave request Module", "User", "0");
                    }
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Overtime request" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Leave request Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        public class CheckedOTRequestListParam
        {
            public string[]? Id { get; set; }
        }
        public partial class TblOvertimeVM
        {
            public string Id { get; set; }
            public string? OTNo { get; set; }
            public string? EmployeeNo { get; set; }
            public DateTime? Date { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? HoursFiled { get; set; }
            public string? HoursApproved { get; set; }
            public string? Remarks { get; set; }
            public string? ConvertToLeave { get; set; }
            public string? ConvertToOffset { get; set; }
            public string? DateCreated { get; set; }
            public int? LeaveId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> CheckedOTRequestList(CheckedOTRequestListParam data)
        {
            try
            {
                //var filter = new CheckedLeaveRequestListParam;
                string sql = "";
                //var result = _context.TblLeaveRequestModel.Where(a => a.isDeleted == false).ToList();
                var result = (dynamic)null;
                sql = $@" SELECT * FROM [TblOvertimeModel] ";
                sql += " WHERE Id in (";
                for (int x = 0; x < data.Id.Length; x++)
                {
                    sql += data.Id[x] + ',';
                }
                sql += "'0')";
                result = new List<TblOvertimeVM>();
                DataTable table = db.SelectDb(sql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    var item = new TblOvertimeVM();
                    item.Id = dr["Id"].ToString();
                    item.OTNo = dr["OTNo"].ToString();
                    item.EmployeeNo = dr["EmployeeNo"].ToString();
                    item.Date = Convert.ToDateTime(dr["Date"]);
                    item.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    item.EndDate = Convert.ToDateTime(dr["EndDate"]);
                    item.StartTime = dr["StartTime"].ToString();
                    item.EndTime = dr["EndTime"].ToString();
                    item.HoursFiled = dr["HoursFiled"].ToString();
                    item.Remarks = dr["Remarks"].ToString();
                    item.ConvertToLeave = dr["ConvertToLeave"].ToString();
                    item.ConvertToOffset = dr["ConvertToOffset"].ToString();
                    result.Add(item);
                }
                //result = result.Where(a => filter.Id?.Contains(a) == true).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public partial class TblOvertimeImportModel
        {
            
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
            public string? ConvertToOffset { get; set; }
            public string? DateCreated { get; set; }
            public int? LeaveId { get; set; }
            public int? Status { get; set; }
            public string? CreatedBy { get; set; }
            public string? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public string? DateDeleted { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Import(List<TblOvertimeImportModel> list)
        {
            string result = "";
            string status = "";
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string query = $@"INSERT INTO [TblOvertimeModel] 
                     ([EmployeeNo],[Date],[StartTime],[EndTime],[StartDate],[EndDate],[HoursFiled],[Remarks],[ConvertToLeave],[ConvertToOffset],[DateCreated],[isDeleted],[CreatedBy],[Status])
                     VALUES ('" + list[0].EmployeeNo + "','" + list[i].Date + "','" + list[i].StartTime + "','" + list[i].EndTime + "','" + list[i].StartDate + "','" + list[i].EndDate + "','" + list[i].HoursFiled + "','" + list[i].Remarks + "','" + list[i].ConvertToLeave + "','" + list[i].ConvertToOffset + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','0','" + list[0].CreatedBy + "','1004');";

                    db.AUIDB_WithParam(query);
                    status = "Inserted Successfully";


                }



            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());

            }

            return Content(status);
        }

    }
}
