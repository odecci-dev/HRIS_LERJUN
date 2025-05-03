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
using MimeKit;
using MailKit.Net.Smtp;

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
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> OvertTimeList(EmployeeIdFilter data)
        {
            try
            {
                DateTime startD = DateTime.ParseExact(data.StartDate, "yyyy-MM-dd", null);
                DateTime endD = DateTime.ParseExact(data.EndDate, "yyyy-MM-dd", null);
                var result = from ot in _context.TblOvertimeModel
                             join leave in _context.TblLeaveTypeModel
                             on ot.LeaveId equals leave.Id into leavegroup
                             from leave in leavegroup.DefaultIfEmpty()
                             join status in _context.TblStatusModels
                             on ot.Status equals status.Id into statusgroup
                             from status in statusgroup.DefaultIfEmpty()
                             where ot.isDeleted == false && ot.EmployeeNo == data.EmployeeNo && ot.Date >= startD && ot.Date <= endD
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
                             Status = ot.Status
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

                        else if (data.Status == 2)
                        {
                            existingItem.Status = 5;

                            string query = $@"UPDATE TblOvertimeModel
	                            SET isDeleted = '1'"
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
        public class NewOverTimeNotificationParam
        {
            public string employeeId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> NewOverTimeNotification(NewOverTimeNotificationParam data)
        {
            var overtime = (from ot in _context.TblOvertimeModel
                            join leave in _context.TblLeaveTypeModel
                            on ot.LeaveId equals leave.Id into leavegroup
                            from leave in leavegroup.DefaultIfEmpty()

                            join employee in _context.TblUsersModels
                            on ot.EmployeeNo equals employee.EmployeeId into employeegroup
                            from employee in employeegroup.DefaultIfEmpty()


                            join manager in _context.TblUsersModels
                            on employee.ManagerId equals manager.Id into managergroup
                            from manager in managergroup.DefaultIfEmpty()

                            join status in _context.TblStatusModels
                            on ot.Status equals status.Id into statusgroup
                            from status in statusgroup.DefaultIfEmpty()

                            where ot.isDeleted == false && ot.EmployeeNo == data.employeeId
                            orderby ot.Date descending // Or whichever column you want to sort by
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
                                LeaveName = leave != null ? leave.Name : "No Leave",
                                LeaveRemarks = leave != null ? leave.Remarks : "",
                                StatusName = status != null ? status.Status : "Unknown",
                                ot.Status,
                                Fullname = employee.Fullname,
                                Email = employee.Email,
                                ManagerName = manager.Fullname,
                                ManagerEmail = manager.Email

                            }).FirstOrDefault();
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress("Odecci", "info@odecci.com"));
            //url = registrationDomain + "registration?empid=" + data.EmployeeId[x] + "&compid=" + data.CompanyId[x] + "&email=" + data.Email[x];
            message.To.Add(new MailboxAddress(overtime.Fullname, overtime.Email));
            message.To.Add(new MailboxAddress(overtime.ManagerName, overtime.ManagerEmail));
            message.To.Add(new MailboxAddress("John Alfred Abalos", "john.abalos@odecci.com"));
            message.To.Add(new MailboxAddress("Odecci Payroll", "payroll@odecci.com"));
            message.To.Add(new MailboxAddress("Ann Santos", "ann.santos@odecci.com"));
            //var recipients = data.Name.Zip(data.Email, (name, email) => new MailboxAddress(name, email)).ToList();

            // Add all recipients at once
            //message.To.AddRange(recipients);

            message.Subject = "Overtime Filing Notification";
            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = @"
                                    <html>
                                        <head>
                                        <meta charset='UTF-8' />
                                        <title>Overtime Filing Notification</title>
                                        </head>
                                        <body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
                                        <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 30px 0;'>
                                            <tr>
                                            <td align='center'>
                                                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 0 10px rgba(0,0,0,0.08);'>
            
                                                <!-- Header -->
                                                <tr>
                                                    <td style='background-color: #205375; padding: 20px; color: #ffffff; text-align: center; font-size: 20px; font-weight: bold;'>
                                                    Overtime Filing Notification
                                                    </td>
                                                </tr>
                                                <!-- Body -->
                                                <tr>
                                                    <td style='padding: 30px; color: #333333; font-size: 16px; line-height: 1.6;'>
                                                    <p style='margin-top: 0;'>Hello Team,</p>
                                                    <p><strong>"+ overtime.Fullname + "</strong> has submitted an overtime request. Please see the details below:</p>"

                                                    + "<!-- List-style Details -->"
                                                    + "<table width='100%' cellpadding='8' cellspacing='0' style='font-size: 14px;'>"
                                                        + "<tr>"
                                                        + "<td width='40%' style='font-weight: bold;'>OT-Number:</td>"
                                                        + "<td>"+ overtime.OTNo + "</td>"
                                                        + "</tr>"
                                                        + "<tr style='background-color: #f9f9f9;'>"
                                                        + "<td style='font-weight: bold;'>Date:</td>"
                                                        + "<td>"+ overtime.Date + "</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                        + "<td style='font-weight: bold;'>Start Date:</td>"
                                                        + "<td>" + overtime.StartDate + "</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                        + "<td style='font-weight: bold;'>Start Time:</td>"
                                                        + "<td>"+ overtime.StartTime + "</td>"
                                                        + "</tr>"
                                                        + "<tr style='background-color: #f9f9f9;'>"
                                                        + "<td style='font-weight: bold;'>End Date:</td>"
                                                        + "<td>" + overtime.EndDate + "</td>"
                                                        + "</tr>"
                                                        + "<tr style='background-color: #f9f9f9;'>"
                                                        + "<td style='font-weight: bold;'>End Time:</td>"
                                                        + "<td>"+ overtime.EndTime + "</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                        + "<td style='font-weight: bold;'>Hours Filed:</td>"
                                                        + "<td>"+ overtime.HoursFiled + "</td>"
                                                        + "</tr>"
                                                        + "<tr style='background-color: #f9f9f9;'>"
                                                        + "<td style='font-weight: bold;'>Reason:</td>"
                                                        + "<td>"+ overtime.Remarks +"</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                        + "<td style='font-weight: bold;'>Convert To Leave:</td>"
                                                        + "<td>"+ overtime.ConvertToLeave + "</td>"
                                                        + "</tr>"
                                                        + "<tr style='background-color: #f9f9f9;'>"
                                                        + "<td style='font-weight: bold;'>Convert To Offset:</td>"
                                                        + "<td>"+ overtime.ConvertToOffset + "</td>"
                                                        + "</tr>"
                                                        + "<tr>"
                                                        + "<td style='font-weight: bold;'>Status:</td>"
                                                        + "<td>"+ overtime.StatusName + "</td>"
                                                        + "</tr>"
                                                    + "</table>"

                                                    + "<!-- CTA Button -->"
                                                    + "<p style='text-align: center; margin: 30px 0;'>"
                                                        + "<a href='https://eportal.odeccisolutions.com/Approval/' style='background-color: #EC1C24; color: white; padding: 12px 24px; border-radius: 5px; text-decoration: none; font-weight: bold;'>Review & Approve</a>"
                                                    + "</p>"
                                                    + "</td>"
                                                + "</tr>"

                                                + "<!-- Footer -->"
                                                + "<tr>"
                                                    + "<td style='background-color: #f0f0f0; text-align: center; padding: 15px; font-size: 12px; color: #777777;'>"
                                                    + "&copy; 2025 Odecci Solution Inc. All rights reserved."
                                                    + "</td>"
                                                + "</tr>"

                                                + "</table>"
                                            + "</td>"
                                            + "</tr>"
                                        + "</table>"
                                        + "</body>"
                                    + "</html>";
            message.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("info@odecci.com", "Roq30573");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            return Ok();
        }

    }
}
