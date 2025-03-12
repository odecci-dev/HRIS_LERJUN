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
using System.Data;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class TimeLogsController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public TimeLogsController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        public class TimeLogsParam
        {
            public string? Usertype { get; set; }
            public string? UserId { get; set; }
            public string? datefrom { get; set; }
            public string? dateto { get; set; }
            public string? Department { get; set; }
        }
        public class CheckBreakTimeParameter
        {
            public int userId { get; set; }
            public string? TimeOfDay { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> CheckBreakTime(CheckBreakTimeParameter tblTimeLog)
        {
            var lastTimein = _context.TblTimeLogs.AsNoTracking().Where(timeLogs => timeLogs.UserId == tblTimeLog.userId && timeLogs.TimeOut == null && timeLogs.StatusId != 5).OrderByDescending(timeLogs => timeLogs.UserId).ToList();

            return Ok(lastTimein);
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
            if (data.datefrom == null)
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
        public partial class BreakTimeParam
        {
            public int? UserId { get; set; }
            public int? Meridiem { get; set; }

        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> BreakTime(BreakTimeParam tblTimeLog)
        {

            var lastTimein = _context.TblTimeLogs.AsNoTracking().Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId && timeLogs.TimeIn != null && timeLogs.TimeOut == null).OrderByDescending(timeLogs => timeLogs.UserId).FirstOrDefault();
            if (lastTimein != null)
            {
                if (lastTimein.TimeOut.IsNullOrEmpty())
                {
                    if (tblTimeLog.Meridiem == 0)
                    {
                        lastTimein.BreakInAm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        _context.Entry(lastTimein).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else if (tblTimeLog.Meridiem == 1)
                    {
                        lastTimein.BreakInPm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        _context.Entry(lastTimein).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        lastTimein.LunchIn = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        _context.Entry(lastTimein).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }

                }
                return Ok("BreakIn");
            }
            else
            {
                return BadRequest("Error!");
            }

        }

        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> RegularTimeIn(TblTimeLog data)
        {
            var lastTimein = _context.TblTimeLogs.AsNoTracking()
                    .Where(timeLogs => timeLogs.UserId == data.UserId && timeLogs.TimeIn != null && timeLogs.TimeOut == null)
                    .OrderByDescending(timeLogs => timeLogs.UserId).FirstOrDefault();
            if (lastTimein == null)
            {
                data.Date = DateTime.Now.Date;
                data.TimeIn = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                data.TimeOut = null;
                data.Remarks = data.Remarks;
                data.TaskId = data.TaskId;
                data.DeleteFlag = 1;
                data.Identifier = "Auto";
                data.StatusId = data.StatusId;
                _context.TblTimeLogs.Add(data);
                await _context.SaveChangesAsync();
            }
            else
            {

                if (lastTimein.BreakInAm != null && lastTimein.BreakOutAm == null)
                {
                    // Parse the string to DateTime
                    DateTime date = DateTime.Parse(lastTimein.BreakInAm);
                    string formattedTime = date.ToString("yyyy-MM-ddTHH:mm");
                    string todate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    lastTimein.BreakOutAm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    DateTime lastTi = DateTime.Parse(formattedTime);
                    DateTime datetimeToday = DateTime.Parse(todate);
                    TimeSpan times = datetimeToday.Subtract(lastTi);
                    double decimalHours = Math.Round(times.TotalHours, 2);
                    lastTimein.TotalBreakAmHours = decimal.Parse(decimalHours.ToString("F2"));
                    _context.Entry(lastTimein).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else if (lastTimein.LunchIn != null && lastTimein.LunchOut == null)
                {
                    if (lastTimein.BreakInAm == null)
                    {
                        lastTimein.BreakInAm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

                    }
                    if (lastTimein.BreakOutAm == null)
                    {
                        lastTimein.BreakOutAm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        // Parse the string to DateTime
                        DateTime Breakdate = DateTime.Parse(lastTimein.BreakInAm);
                        string BreakformattedTime = Breakdate.ToString("yyyy-MM-ddTHH:mm");
                        string Breaktodate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        DateTime BreaklastTi = DateTime.Parse(BreakformattedTime);
                        DateTime BreakdatetimeToday = DateTime.Parse(Breaktodate);
                        TimeSpan Breaktimes = BreakdatetimeToday.Subtract(BreaklastTi);
                        double BreakdecimalHours = Math.Round(Breaktimes.TotalHours, 2);
                        lastTimein.TotalBreakAmHours = decimal.Parse(BreakdecimalHours.ToString("F2"));
                    }
                    // Parse the string to DateTime
                    DateTime date = DateTime.Parse(lastTimein.LunchIn);
                    string formattedTime = date.ToString("yyyy-MM-ddTHH:mm");
                    string todate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    lastTimein.LunchOut = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    DateTime lastTi = DateTime.Parse(formattedTime);
                    DateTime datetimeToday = DateTime.Parse(todate);
                    TimeSpan times = datetimeToday.Subtract(lastTi);
                    double decimalHours = Math.Round(times.TotalHours, 2);
                    lastTimein.TotalLunchHours = decimal.Parse(decimalHours.ToString("F2"));
                    _context.Entry(lastTimein).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (lastTimein.LunchIn == null)
                    {
                        lastTimein.LunchIn = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

                    }
                    if (lastTimein.LunchOut == null)
                    {
                        lastTimein.LunchOut = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        // Parse the string to DateTime
                        DateTime Breakdate = DateTime.Parse(lastTimein.LunchIn);
                        string BreakformattedTime = Breakdate.ToString("yyyy-MM-ddTHH:mm");
                        string Breaktodate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        DateTime BreaklastTi = DateTime.Parse(BreakformattedTime);
                        DateTime BreakdatetimeToday = DateTime.Parse(Breaktodate);
                        TimeSpan Breaktimes = BreakdatetimeToday.Subtract(BreaklastTi);
                        double BreakdecimalHours = Math.Round(Breaktimes.TotalHours, 2);
                        lastTimein.TotalLunchHours = decimal.Parse(BreakdecimalHours.ToString("F2"));

                    }
                    // Parse the string to DateTime
                    DateTime date = DateTime.Parse(lastTimein.BreakInPm);
                    string formattedTime = date.ToString("yyyy-MM-ddTHH:mm");
                    string todate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    lastTimein.BreakOutPm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    DateTime lastTi = DateTime.Parse(formattedTime);
                    DateTime datetimeToday = DateTime.Parse(todate);
                    TimeSpan times = datetimeToday.Subtract(lastTi);
                    double decimalHours = Math.Round(times.TotalHours, 2);
                    lastTimein.TotalBreakPmHours = decimal.Parse(decimalHours.ToString("F2"));
                    _context.Entry(lastTimein).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

            }

            return Ok();
        }
        [HttpPost]
        public async Task<ActionResult<TblTimeLog>> RegularTimeOut(User tblTimeLog)
        {

            var lastTimein = _context.TblTimeLogs.AsNoTracking().Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId && timeLogs.TimeIn != null && timeLogs.TimeOut == null).OrderByDescending(timeLogs => timeLogs.UserId).FirstOrDefault();
            if (lastTimein != null)
            {

                if (lastTimein.TimeOut.IsNullOrEmpty())
                {
                    if (lastTimein.BreakInPm == null)
                    {
                        lastTimein.BreakInPm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");

                    }
                    if (lastTimein.BreakOutPm == null)
                    {
                        lastTimein.BreakOutPm = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        // Parse the string to DateTime
                        DateTime Breakdate = DateTime.Parse(lastTimein.BreakInPm);
                        string BreakformattedTime = Breakdate.ToString("yyyy-MM-ddTHH:mm");
                        string Breaktodate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                        DateTime BreaklastTi = DateTime.Parse(BreakformattedTime);
                        DateTime BreakdatetimeToday = DateTime.Parse(Breaktodate);
                        TimeSpan Breaktimes = BreakdatetimeToday.Subtract(BreaklastTi);
                        double BreakdecimalHours = Math.Round(Breaktimes.TotalHours, 2);
                        lastTimein.TotalBreakPmHours = decimal.Parse(BreakdecimalHours.ToString("F2"));
                    }
                    // Parse the string to DateTime
                    DateTime date = DateTime.Parse(lastTimein.TimeIn);
                    string formattedTime = date.ToString("yyyy-MM-ddTHH:mm");
                    string todate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    lastTimein.TimeOut = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    DateTime lastTi = DateTime.Parse(formattedTime);
                    DateTime datetimeToday = DateTime.Parse(todate);
                    TimeSpan times = datetimeToday.Subtract(lastTi);
                    double decimalHours = Math.Round(times.TotalHours, 2);
                    lastTimein.RenderedHours = decimal.Parse(decimalHours.ToString("F2")) - lastTimein.TotalLunchHours;
                    _context.Entry(lastTimein).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
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
        [HttpPost]
        public async Task<IActionResult> RegularTimeLogList(TimeLogsParam data)
        {
            try
            {
                var result = _context.TblTimeLogs
                                .Join(_context.TblUsersModels, tl => tl.UserId, um => um.Id, (tl, um) => new { tl, um })
                                .Join(_context.TblEmployeeTypes, tlu => tlu.um.EmployeeType, et => et.Id, (tlu, et) => new { tlu.tl, tlu.um, et })
                                .Join(_context.TblScheduleModels, tluet => tluet.et.ScheduleId, sched => sched.Id, (tluet, sched) => new { tluet.tl, tluet.um, tluet.et, sched })
                                .Join(_context.TblTimeLogsStatus, tluets => tluets.tl.StatusId, tls => tls.StatusId, (tluets, tls) => new { tluets.tl, tluets.um, tluets.et, tluets.sched, tls })
                                .Join(_context.TblTaskModels, tluetst => tluetst.tl.TaskId, tm => tm.Id, (tluetst, tm) => new { tluetst.tl, tluetst.um, tluetst.et, tluetst.sched, tluetst.tls, tm })
                                .OrderByDescending(t => t.tl.Id)
                                .AsEnumerable() // Forces LINQ to execute in memory
                                .Select(t =>
                                {
                                    // Convert TimeIn to DateTime
                                    bool isTimeInValid = DateTime.TryParse(t.tl.TimeIn, out DateTime timeIn);
                                    // Convert MondayS to DateTime (Time-Only)
                                    bool isMondaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime mschedTime);
                                    bool isTuesdaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime tschedTime);
                                    bool isWednesdaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime wschedTime);
                                    bool isThusdaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime thschedTime);
                                    bool isFridaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime fschedTime);
                                    bool isSaturdaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime satschedTime);
                                    bool isSundaySValid = DateTime.TryParseExact(t.sched.MondayS, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime sunschedTime);

                                    return new
                                    {
                                        id = t.tl.Id,
                                        userId = t.tl.UserId,
                                        date = t.tl.Date,
                                        timeIn = t.tl.TimeIn,
                                        timeOut = t.tl.TimeOut,
                                        renderedHours = t.tl.RenderedHours,
                                        username = t.um.Username,
                                        fname = t.um.Fname,
                                        lname = t.um.Lname,
                                        mname = t.um.Mname,
                                        suffix = t.um.Suffix,
                                        email = t.um.Email,
                                        employeeID = t.um.EmployeeId,
                                        JWToken = t.um.Jwtoken,
                                        filePath = t.um.FilePath,
                                        usertypeId = t.um.UserType,
                                        lunchIn = t.tl.LunchIn,
                                        lunchOut = t.tl.LunchOut,
                                        totalLunchHours = t.tl.TotalLunchHours,
                                        breakInAm = t.tl.BreakInAm,
                                        breakOutAm = t.tl.BreakOutAm,
                                        totalBreakAmHours = t.tl.TotalBreakAmHours,
                                        breakInPm = t.tl.BreakInPm,
                                        breakOutPm = t.tl.BreakOutPm,
                                        totalBreakPmHours = t.tl.TotalBreakPmHours,
                                        statusId = t.tl.StatusId,
                                        statusName = t.tls.Status,
                                        deleteFlag = t.tl.DeleteFlag,
                                        identifier = t.tl.Identifier,
                                        remarks = t.tl.Remarks,
                                        taskId = t.tl.TaskId,
                                        dateCreated = t.tl.DateCreated,
                                        dateUpdated = t.tl.DateUpdated,
                                        dateDeleted = t.tl.DateDeleted,
                                        employeeType = t.um.EmployeeType,
                                        task = t.tm.Title,
                                        isUnderTime = t.tl.RenderedHours >= 8 ? "0" : "1",
                                        timelogStatus = t.tl.Date.HasValue
                                            ? t.tl.Date.Value.DayOfWeek switch
                                            {
                                                DayOfWeek.Monday => t.sched.MondayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isMondaySValid
                                                        ? (timeIn.TimeOfDay > mschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                DayOfWeek.Tuesday => t.sched.TuesdayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isTuesdaySValid
                                                        ? (timeIn.TimeOfDay > tschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                DayOfWeek.Wednesday => t.sched.WednesdayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isWednesdaySValid
                                                        ? (timeIn.TimeOfDay > wschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                DayOfWeek.Thursday => t.sched.ThursdayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isThusdaySValid
                                                        ? (timeIn.TimeOfDay > thschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                DayOfWeek.Friday => t.sched.FridayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isFridaySValid
                                                        ? (timeIn.TimeOfDay > fschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                DayOfWeek.Saturday => t.sched.SaturdayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isSaturdaySValid
                                                        ? (timeIn.TimeOfDay > satschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                DayOfWeek.Sunday => t.sched.SundayS == null
                                                    ? "Rest day OT"
                                                    : (isTimeInValid && isSundaySValid
                                                        ? (timeIn.TimeOfDay > sunschedTime.TimeOfDay ? "Late" : "ONTIME")
                                                        : "Invalid Time"),
                                                _ => "Unknown"
                                            }
                                            : "Unknown"
                                    };
                                })
                                .ToList(); // Convert to List to finalize processing





                //var result = (dynamic)null;
                //result = _context.TblOvertimeModel.Where(a => a.isDeleted == false && a.EmployeeNo == data.EmployeeNo).OrderByDescending(a => a.Id).ToList();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
