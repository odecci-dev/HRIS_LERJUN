using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using HonkSharp.Fluency;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class TimeLogsController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        DBMethods dbmet = new DBMethods();

        public TimeLogsController(ODC_HRISContext context)
        {
            _context = context;
        }

        public class TimeLogsParam
        {
            public string? Usertype { get; set; }
            public string? UserId { get; set; }
            public string? datefrom { get; set; }
            public string? dateto { get; set; }
            public string? Department { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> TimeLogsList(TimeLogsParam data)
        {
            var result = (dynamic)null;
            var validation = dbmet.TimeLogsData().Where(a => a.UserId == data.UserId).FirstOrDefault();
            if (validation != null)
            {
                //
                if (validation.UsertypeId != "2")
                {
                    result = dbmet.TimeLogsData().Where(a => a.UserId == data.UserId && Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom) && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.Id).ToList();
                }
                else
                {
                    result = dbmet.TimeLogsData().Where(a => Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom) && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.Id).ToList();
                }
            }
            else
            {
                return BadRequest("ERROR");
            }


            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> TimeLogsPending(TimeLogsParam data)
        {
            var result = (dynamic)null;
            if (data.UserId == "0")
            {
                result = dbmet.TimeLogsData().Where(a => a.StatusId == "0").OrderByDescending(a => a.Id).ToList();

            }
            else
            {
                result = dbmet.TimeLogsData().Where(a => a.StatusId == "0" && a.UserId == data.UserId).OrderByDescending(a => a.Id).ToList();
            }


            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> NotificationList(TblNotification data)
        {
            var result = (dynamic)null;
            var validation = _context.TblTimeLogNotifications.ToList();
            if (validation != null)
            {
                //
                if (data.StatusId == null)
                {
                    result = _context.TblTimeLogNotifications.OrderByDescending(a => a.Id).ToList();
                }
                else
                {
                    result = _context.TblTimeLogNotifications.Where(a => a.StatusId == data.StatusId).OrderByDescending(a => a.Id).ToList();
                }
            }
            else
            {
                return BadRequest("ERROR");
            }


            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> NotificationUnreadCount()
        {
            var result = (dynamic)null;
            var validation = _context.TblTimeLogNotifications.ToList();
            if (validation != null)
            {
                result = _context.TblTimeLogNotifications.Where(a => a.StatusId == 3).Count();
            }
            else
            {
                return BadRequest("ERROR");
            }


            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> TimeLogsListManager(TimeLogsParam data)
        {
            var result = (dynamic)null;
            if(data.datefrom == null)
            {
                result = dbmet.TimeLogsData().OrderByDescending(a => a.Id).ToList();
            }
            else
            {
                if (data.Department == "0" && data.UserId == "0")
                {
                    result = dbmet.TimeLogsData().Where(a => Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom) && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.Id).ToList();

                }
                else if (data.Department != "0" && data.UserId == "0")
                {
                    result = dbmet.TimeLogsData().Where(a => Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom) && a.Department == data.Department && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.Id).ToList();

                }
                else if (data.Department == "0" && data.UserId != "0")
                {
                    result = dbmet.TimeLogsData().Where(a => Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom) && a.UserId == data.UserId && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.Id).ToList();

                }
                else if (data.Department != "0" && data.UserId != "0")
                {
                    result = dbmet.TimeLogsData().Where(a => Convert.ToDateTime(a.Date) >= Convert.ToDateTime(data.datefrom) && a.UserId == data.UserId && a.Department == data.Department && Convert.ToDateTime(a.Date) <= Convert.ToDateTime(data.dateto)).OrderByDescending(a => a.Id).ToList();

                }
            }
           


            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> TimeIn(TblTimeLog data)
        {

            data.Date = DateTime.Now.Date;
            //data.TimeIn = DateTime.Now.ToString("hh:mm:ss tt");

            data.TimeIn = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            data.TimeOut = null;
            data.Remarks = data.Remarks;
            data.TaskId = data.TaskId;
            data.DeleteFlag = 1;
            data.Identifier = "Auto";
            data.StatusId = data.StatusId;
            _context.TblTimeLogs.Add(data);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> ManualLogs(TblTimeLog data)
        {
            //var result = _context.TblTimeLogs.Where(a => a.Id == data.Id).OrderByDescending(a => a.Id).ToList();
            if (data.Id == 0)
            {
                var item = new TblTimeLog();
                item.Id = data.Id;
                item.UserId = data.UserId;
                item.Date = data.Date;
                item.TimeIn = data.TimeIn;
                item.TimeOut = data.TimeOut;
                item.RenderedHours = data.RenderedHours;
                item.DeleteFlag = 1;
                item.StatusId = 0;
                item.Identifier = "Manual";
                item.Remarks = data.Remarks;
                item.TaskId = data.TaskId;
                item.DateCreated = DateTime.Now.Date;
                item.DateUpdated = null;
                item.DateDeleted = null;
                _context.TblTimeLogs.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {

                //var item = new TblTimeLog();
                //item.Id = data.Id;
                //item.UserId = data.UserId;
                //item.Date = data.Date;
                //item.TimeIn = data.TimeIn;
                //item.TimeOut = data.TimeOut;
                //item.RenderedHours = data.RenderedHours;
                //item.StatusId = 0;
                //item.DeleteFlag = 1;
                //item.Identifier = "Manual";
                //item.Remarks = data.Remarks;
                //item.TaskId = data.TaskId;
                //item.DateCreated = result[0].DateCreated;
                //item.DateUpdated = DateTime.Now.Date;
                //item.DateDeleted = result[0].DateDeleted;
                DateTime getDateforUpdate = DateTime.Now.Date;
                var currentDate = data.Date.ToString();
                DateTime getDate = DateTime.Parse(currentDate);
                string formattedDate = getDate.ToString("yyyy-MM-dd");

                string formattedUpdateDate = getDateforUpdate.ToString("yyyy-MM-dd");
                string query = $@"UPDATE [tbl_TimeLogs]
                                    SET StatusId = 0,
                                    Date = '" + formattedDate + "',"
                                    + "TimeIn = '" + data.TimeIn + "',"
                                    + "TimeOut = '" + data.TimeOut + "',"
                                    + "TaskId = '" + data.TaskId + "',"
                                    + "RenderedHours = '" + data.RenderedHours + "',"
                                    + "DateUpdated = '" + formattedUpdateDate + "',"
                                    + "DeleteFlag = '" + data.DeleteFlag + "',"
                                    + "Remarks = '" + data.Remarks + "' "
                                    + " WHERE Id = '" + data.Id + "'";
                db.AUIDB_WithParam(query);
                //_context.Entry(item).State = EntityState.Modified;
            }
            return Ok();
        }
        public class TimeLogId
        {
            public int Id { get; set; }
            public int Action { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> UpdateLogStatus(TimeLogId data)
        {

            var Status = 0;
            try
            {

                if (data.Action == 0)
                {
                    Status = 1;
                }
                else
                {
                    Status = 2;
                }
                string query = $@"UPDATE [tbl_TimeLogs]
                                    SET StatusId = " + Status
                                + " WHERE Id = '" + data.Id + "'";
                db.AUIDB_WithParam(query);
                //_context.Entry(item).State = EntityState.Modified;
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> LogsNotification(TblNotification data)
        {
            if (data.Id == 0)
            {
                var item = new TblNotification();
                item.Id = data.Id;
                item.Notification = data.Notification;
                item.UserId = data.UserId;
                item.Date = data.Date;
                item.StatusId = data.StatusId;
                _context.TblTimeLogNotifications.Add(item);
            }
            else
            {
                var item = new TblNotification();
                item.Id = data.Id;
                item.Notification = data.Notification;
                item.UserId = data.UserId;
                item.Date = data.Date;
                item.StatusId = data.StatusId;
                _context.Entry(item).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> TimeOut(User tblTimeLog)
        {

            var lastTimein = _context.TblTimeLogs.AsNoTracking().Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId && timeLogs.TimeIn != null && timeLogs.TimeOut == null).OrderByDescending(timeLogs => timeLogs.UserId).FirstOrDefault();
            if (lastTimein != null)
            {

                if (lastTimein.TimeOut.IsNullOrEmpty())
                {
                    // Parse the string to DateTime
                    DateTime date = DateTime.Parse(lastTimein.TimeIn);
                    string formattedTime = date.ToString("yyyy-MM-ddTHH:mm");
                    string todate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    //lastTimein.TimeOut = DateTime.Now.ToString("hh:mm:ss tt");
                    lastTimein.TimeOut = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    DateTime lastTi = DateTime.Parse(formattedTime);
                    DateTime datetimeToday = DateTime.Parse(todate);
                    TimeSpan times = datetimeToday.Subtract(lastTi);
                    //lastTimein.RenderedHours = decimal.Parse(times.Hours.ToString() + "." + times.Minutes.ToString());
                    double decimalHours = Math.Round(times.TotalHours, 2);
                    lastTimein.RenderedHours = decimal.Parse(decimalHours.ToString("F2"));
                    _context.Entry(lastTimein).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    //string query = $@"UPDATE [tbl_TimeLogs]"
                    //                + "TimeOut = '" + lastTimein.TimeOut + "',"
                    //                + "RenderedHours = '" + lastTimein.RenderedHours 
                    //                + " WHERE Id = '" + lastTimein.Id + "'";
                    //db.AUIDB_WithParam(query);
                }
                return Ok("TimeOut");
            }
            else
            {
                return BadRequest("Error!");
            }

        }
        public partial class User
        {
            public int? UserId { get; set; }

            //

        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> getLastTimeIn(User tblTimeLog)
        {
            bool lastTimein = true;
            var validation = _context.TblTimeLogs.Where(a => a.UserId == tblTimeLog.UserId).ToList();
            if (validation.Count() != 0)
            {
                lastTimein = _context.TblTimeLogs.AsNoTracking().Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId && timeLogs.TimeOut == null && timeLogs.StatusId != 5).OrderByDescending(timeLogs => timeLogs.UserId).ToList().Count() > 0;

            }
            else
            {
                lastTimein = false;

            }

            return Ok(lastTimein);
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> save(string type, TblTimeLog tblTimeLog)
        {
            if (_context.TblTimeLogs == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblTimeLogs'  is null.");
            }
            //bool hasDuplicateOnSave = (_context.TblUsersModels?.Any(userModel => userModel.Email == tblUserModel.Email)).GetValueOrDefault();
            bool isExist = (_context.TblUsersModels?.Any(userModel => userModel.Id == tblTimeLog.UserId && !userModel.DeleteFlag)).GetValueOrDefault();

            if (!isExist)
            {
                return Conflict("User Id does not Exist");
            }

            try
            {
                if (type.ToLower() == "timein")
                {
                    //string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tblTimeLog.Date = DateTime.Now.Date;
                    tblTimeLog.TimeIn = DateTime.Now.ToString("hh:mm:ss tt"); ;
                    tblTimeLog.TimeOut = null;
                    tblTimeLog.DeleteFlag = 1;
                    _context.TblTimeLogs.Add(tblTimeLog);
                    await _context.SaveChangesAsync();
                }
                else if (type.ToLower() == "timeout")
                {
                    var lastTimein = _context.TblTimeLogs.AsNoTracking().Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId).OrderBy(timeLogs => timeLogs.Id).LastOrDefault();
                    if (lastTimein.TimeOut.IsNullOrEmpty())
                    {

                        tblTimeLog.TimeIn = lastTimein.TimeIn.ToString();
                        tblTimeLog.TimeOut = DateTime.Now.ToString("hh:mm:ss tt");
                        tblTimeLog.DeleteFlag = 1;
                        TimeSpan times = DateTime.Parse(tblTimeLog.TimeOut).Subtract(DateTime.Parse(tblTimeLog.TimeIn));
                        tblTimeLog.RenderedHours = decimal.Parse(times.Hours.ToString() + "." + times.Minutes.ToString());
                        _context.Entry(tblTimeLog).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        tblTimeLog.Date = DateTime.Now.Date;
                        tblTimeLog.TimeOut = DateTime.Now.ToString("hh:mm:ss tt");
                        tblTimeLog.TimeIn = null;
                        tblTimeLog.DeleteFlag = 1;
                        _context.TblTimeLogs.Add(tblTimeLog);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    return Conflict("Invalid Type");
                }

                return CreatedAtAction("save", new { id = tblTimeLog.Id }, tblTimeLog);


            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
