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

        public LeaveController(ODC_HRISContext context,DBMethods _dbmet)
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
        }
        [HttpPost]
        public async Task<IActionResult> LeaveRequestList(LeaveRequestListParam data)
        {
            var result = _context.TblLeaveRequestModel.Where(a => a.isDeleted == false).ToList();
            return Ok(result);
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

                    if(data.Status == 0)
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
        [HttpGet]
        public async Task<IActionResult> LeaveTypeList()
        {
            var result = _context.TblLeaveTypeModel.Where(a => a.isDeleted == false).ToList();
            return Ok(result);
        }
    }
}
