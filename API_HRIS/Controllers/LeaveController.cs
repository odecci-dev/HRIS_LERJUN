using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using PeterO.Numbers;
using System.Data;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public LeaveController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        [HttpGet]
        public async Task<IActionResult> TaskList()
        {
            var result = _context.TblTaskModels.Where(a => a.Status == 1 && a.isBreak == 0).ToList();
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> BreakList()
        {
            var result = _context.TblTaskModels.Where(a => a.Status == 1 && a.isBreak == 1 || a.Id == 2).ToList();
            return Ok(result);
        }
        public class LeaveRequestListParam
        {
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? UserId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> LeaveRequestList(LeaveRequestListParam data)
        {
            try
            {
                var result = _context.TblLeaveRequestModel.Where(a => a.isDeleted == false).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public class CheckedLeaveRequestListParam
        {
            public string[]? Id { get; set; }
        }
        public class TblLeaveRequestVM
        {
            public string? Id { get; set; }
            public string? LeaveRequestNo { get; set; }
            public string? EmployeeNo { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? DaysFiled { get; set; }
            public string? LeaveTypeId { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> CheckedLeaveRequestList(CheckedLeaveRequestListParam data)
        {
            try
            {
                //var filter = new CheckedLeaveRequestListParam;
                string sql = "";
                //var result = _context.TblLeaveRequestModel.Where(a => a.isDeleted == false).ToList();
                var result = (dynamic)null;
                sql = $@" SELECT * FROM [TblLeaveRequestModel] ";
                sql += " WHERE Id in (";
                for (int x = 0; x < data.Id.Length; x++)
                {
                    sql += data.Id[x] + ',';
                }
                sql += "'0')";
                result = new List<TblLeaveRequestVM>();
                DataTable table = db.SelectDb(sql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    var item = new TblLeaveRequestVM();
                    item.Id = dr["Id"].ToString();
                    item.LeaveRequestNo = dr["LeaveRequestNo"].ToString();
                    item.EmployeeNo = dr["EmployeeNo"].ToString();
                    item.Date = Convert.ToDateTime(dr["Date"]);
                    item.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    item.EndDate = Convert.ToDateTime(dr["EndDate"]);
                    item.DaysFiled = dr["DaysFiled"].ToString();
                    item.Reason = dr["Reason"].ToString();
                    item.Status = dr["Status"].ToString();
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
        [HttpPost]
        public async Task<IActionResult> PendingLeaveRequestList(LeaveRequestListParam data)
        {
            try
            {
                var result = _context.TblLeaveRequestModel.Where(a => a.isDeleted == false && a.Status == 1004).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<TblLeaveRequestModel>> Save(TblLeaveRequestModel data)
        {

            //bool hasDuplicateOnSave = (_context.TblLeaveRequestModel?.Any(a => a.LeaveRequestNo == data.TeamName)).GetValueOrDefault();
            string status = "";
            //if (hasDuplicateOnSave)
            //{
            //    return Conflict("Entity already exists");
            //}
            var leaveno = "";
            var firstRecord = _context.TblLeaveRequestModel.OrderByDescending(x => x.Id).FirstOrDefault();
            var no = 2025 * 100 + firstRecord.Id;
            if (firstRecord == null)
            {
                leaveno = "LR202501";
            }
            else
            {
                leaveno = "LR" + no;
            }
            try
            {

                if (data.Id == 0)
                { // Insert Team

                    var leaveRequest = new TblLeaveRequestModel
                    {
                        LeaveRequestNo = leaveno,
                        EmployeeNo = data.EmployeeNo,
                        Date = DateTime.Now,
                        StartDate = data.StartDate,
                        EndDate = data.EndDate,
                        DaysFiled = data.DaysFiled,
                        LeaveTypeId = data.LeaveTypeId,
                        Reason = data.Reason,
                        Status = 1004,
                        isDeleted = false, // Default Active
                        CreatedBy = data.CreatedBy,
                        DateCreated = DateTime.Now,
                    };
                    _context.TblLeaveRequestModel.Add(leaveRequest);
                    await _context.SaveChangesAsync();

                    await _context.SaveChangesAsync();
                    status = "Leave Request successfully saved";
                    dbmet.InsertAuditTrail("Save Leave Request" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Leave Request Module", "User", "0");
                }
                else
                {
                    // Update Existing Team
                    var existingItem = await _context.TblLeaveRequestModel.FindAsync(data.Id);
                    if (existingItem != null)
                    {
                        existingItem.StartDate = data.StartDate;
                        existingItem.EndDate = data.EndDate;
                        existingItem.LeaveTypeId = data.LeaveTypeId; // Assuming CreatedBy is the user updating
                        existingItem.UpdatedBy = data.UpdatedBy;
                        existingItem.DateUpdated = DateTime.Now;
                        existingItem.Reason = data.Reason;
                        existingItem.Status = 1004;
                        existingItem.DaysFiled = data.DaysFiled;

                        _context.TblLeaveRequestModel.Update(existingItem);
                        await _context.SaveChangesAsync();
                        status = "Leave Request successfully updated";
                        dbmet.InsertAuditTrail("Update Leave Request " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Leave Request Module", "User", data.Id.ToString());
                    }
                }


                return CreatedAtAction("save", new { id = data.Id }, data);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Leave Request" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Leave Request Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        public class LeaveRequestParam
        {
            public int Id { get; set; }
            public int Status { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblLeaveRequestModel>> UpdateStatus(LeaveRequestParam data)
        {

            var isExisting = _context.TblLeaveRequestModel.Where(a => a.Id == data.Id).OrderByDescending(a => a.Id).ToList();
            string status = "";

            try
            {

                var existingItem = await _context.TblLeaveRequestModel.FindAsync(data.Id);
                if (existingItem != null)
                {

                    if (data.Status == 0)
                    {

                        existingItem.Status = 1005;

                    }
                    else if (data.Status == 1)
                    {

                        existingItem.Status = 5;

                    }
                    _context.TblLeaveRequestModel.Update(existingItem);
                    await _context.SaveChangesAsync();
                    status = "Leave request successfully deleted";
                    dbmet.InsertAuditTrail("Update Leave request" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Leave request Module", "User", "0");


                }
                return Ok();
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Leave request" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Leave request Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class multiApprovalParamList
        {
            public int? Id { get; set; }
            public string? reason { get; set; }
        }
        public class multiApprovalParam
        {
            public int? Status { get; set; }
            public List<multiApprovalParamList> lrapproval { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblLeaveRequestModel>> MultiUpdateStatus(multiApprovalParam data)
        {
            string status = "";

            try
            {
                for (int i = 0; i < data.lrapproval.Count; i++)
                {
                    Console.WriteLine(data.lrapproval[i].Id);
                    var existingItem = await _context.TblLeaveRequestModel.FindAsync(data.lrapproval[i].Id);
                    if (existingItem != null)
                    {
                        if (data.Status == 0)
                        {
                            existingItem.Status = 1005;
                        }
                        else if (data.Status == 1)
                        {
                            existingItem.Status = 5;
                        }
                        existingItem.ApprovalReason = data.lrapproval[i].reason;
                        _context.TblLeaveRequestModel.Update(existingItem);
                        await _context.SaveChangesAsync();
                        status = "Leave request successfully deleted";
                        dbmet.InsertAuditTrail("Update Leave request" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Leave request Module", "User", "0");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Leave request" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Leave request Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> LeaveTypeList()
        {
            var result = _context.TblLeaveTypeModel.Where(a => a.isDeleted == false).ToList();
            return Ok(result);
        }
    }
}
