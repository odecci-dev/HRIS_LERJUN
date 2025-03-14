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
            string queryIsLoggedIn = $@"UPDATE [dbo].[tbl_UsersModel] SET [isLoggedIn] = '1'" +
                                       " WHERE id = '" + data.UserId+ "'";
            db.DB_WithParam(queryIsLoggedIn);
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
                string queryIsLoggedIn = $@"UPDATE [dbo].[tbl_UsersModel] SET [isLoggedIn] = '1'" +
                                       " WHERE id = '" + tblTimeLog.UserId + "'";
                db.DB_WithParam(queryIsLoggedIn);
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
                string queryIsLoggedIn = $@"UPDATE [dbo].[tbl_UsersModel] SET [isLoggedIn] = '1'" +
                                       " WHERE id = '" + data.UserId + "'";
                db.DB_WithParam(queryIsLoggedIn);
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

                    string queryIsLoggedIn = $@"UPDATE [dbo].[tbl_UsersModel] SET [isLoggedIn] = '0'" +
                                           " WHERE id = '" + tblTimeLog.UserId + "'";
                    db.DB_WithParam(queryIsLoggedIn);
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
        //[HttpPost]
        //public async Task<ActionResult<TblTimeLog>> getLastTimeIn(User tblTimeLog)
        //{
        //    try { 
        //        bool lastTimeIn = false; // Default value

        //        // Get the last active TimeLog entry for the user
        //        var lastLog = await _context.TblTimeLogs
        //            .AsNoTracking()
        //            .Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId && timeLogs.TimeOut == null && timeLogs.StatusId != 5)
        //            .OrderByDescending(timeLogs => timeLogs.TimeIn) // Assuming TimeIn stores the timestamp
        //            .FirstOrDefaultAsync(); // Fetch the latest record

        //        if (lastLog != null)
        //        {
        //            lastTimeIn = true;
        //        }
        //        else
        //        {
        //            lastTimeIn = false;
        //        }

        //        return Ok(lastTimeIn);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpPost]
        public async Task<ActionResult<bool>> getLastTimeIn(User tblTimeLog)
        {
            try
            {
                if (tblTimeLog == null || tblTimeLog.UserId == null)
                {
                    return BadRequest("UserId cannot be null.");
                }

                // Get the last active TimeLog entry for the user
                var lastLog = await _context.TblTimeLogs
                    .AsNoTracking()
                    .Where(timeLogs => timeLogs.UserId == tblTimeLog.UserId && timeLogs.TimeOut == null && timeLogs.StatusId != 5)
                    .OrderByDescending(timeLogs => timeLogs.TimeIn)
                    .FirstOrDefaultAsync();

                bool lastTimeIn = lastLog != null;

                return Ok(lastTimeIn);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
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


        public class ManagerTimelogsVM
        {
            public string? Id { get; set; }
            public string? UserId { get; set; }
            public string? Date { get; set; }
            public string? TimeIn { get; set; }
            public string? TimeOut { get; set; }
            public string? RenderedHours { get; set; }
            public string? LunchIn { get; set; }
            public string? LunchOut { get; set; }
            public string? TotalLunchHours { get; set; }
            public string? BreakInAm { get; set; }
            public string? BreakOutAm { get; set; }
            public string TotalBreakAmHours { get; set; }
            public string? BreakInPm { get; set; }
            public string? BreakOutPm { get; set; }
            public string? TotalBreakPmHours { get; set; }
            public string? Username { get; set; }
            public string? Fname { get; set; }
            public string? Lname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Email { get; set; }
            public string? EmployeeID { get; set; }
            public string? JWToken { get; set; }
            public string? FilePath { get; set; }
            public string? UsertypeId { get; set; }
            public string? UserType { get; set; }
            public string? DeleteFlagName { get; set; }
            public string? DeleteFlag { get; set; }
            public string? StatusName { get; set; }
            public string? StatusId { get; set; }
            public string? Remarks { get; set; }
            public string? TaskId { get; set; }
            public string? Task { get; set; }
            public string? Department { get; set; }
            public string? TimelogStatus { get; set; }
            public string? IsUnderTime { get; set; }
            public string? EmployeeType { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> RegularTimeLogList(TimeLogsParam data)
        {
            
            var result = (dynamic)null;
            try
            {
                string sql = $@"SELECT 
	                        tl.*
	                        ,t.Title as 'Task'
	                        ,um.Username
	                        ,um.Fname
	                        ,um.Lname
	                        ,um.Mname
	                        ,um.Suffix
	                        ,um.Email
	                        ,um.EmployeeID
	                        ,um.FilePath
	                        ,um.UserType as 'UsertypeId'
	                        ,ts.Status as 'StatusName'
	                        ,um.EmployeeType
	                        ,CASE
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Monday' 
		                        THEN 
			                        CASE
				                        WHEN sched.MondayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.MondayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Tuesday' 
		                        THEN 
			                        CASE
				                        WHEN sched.TuesdayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.TuesdayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Wednesday' 
		                        THEN 
			                        CASE
				                        WHEN sched.WednesdayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.WednesdayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Thursday' 
		                        THEN 
			                        CASE
				                        WHEN sched.ThursdayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.ThursdayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Friday' 
		                        THEN 
			                        CASE
				                        WHEN sched.FridayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.FridayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Saturday' 
		                        THEN 
			                        CASE
				                        WHEN sched.SaturdayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.SaturdayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Sunday' 
		                        THEN 
			                        CASE
				                        WHEN sched.SundayS IS NULL THEN 'RestDayOverTime'
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.SundayS
					                        THEN 'Late'
					                        ELSE 'ONTIME'
				                        END
			                        END
		                        ELSE NULL
	                        END as TimeLogsStatus
	                        ,CASE
		                        WHEN tl.RenderedHours >= 8 THEN '0'
		                        ELSE '1'
	                        END as isUnderTime
                        FROM tbl_TimeLogs tl WITH(NOLOCK)
                        LEFT JOIN tbl_UsersModel um WITH(NOLOCK)
                        ON tl.UserId = um.ID
                        LEFT JOIN tbl_EmployeeType et WITH(NOLOCK)
                        ON et.ID = um.EmployeeType
                        LEFT JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.ID = et.ScheduleId
                        LEFT JOIN tbl_TimeLogStatus ts WITH(NOLOCK)
                        ON ts.StatusId = tl.StatusId
                        LEFT JOIN tbl_TaskModel t WITH(NOLOCK)
                        ON t.Id = tl.TaskId
                        WHERE tl.Date between '" + data.datefrom + "' AND '" + data.dateto+"'";

                
                if (data.Department != "0")
                {
                    sql += " AND um.Department = '" +data.Department +"'";
                }
                if (data.UserId != "0")
                {
                    sql += " AND um.Id = '" + data.UserId + "'";
                }


                sql += " ORDER BY tl.Id desc";
                result = new List<ManagerTimelogsVM>();
                DataTable table = db.SelectDb(sql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    var item = new ManagerTimelogsVM();
                    item.Id = dr["Id"].ToString();
                    item.UserId = dr["UserId"].ToString();
                    item.Date = Convert.ToDateTime(dr["Date"].ToString()).ToString("MM-dd-yyyy");
                    item.TimeIn = dr["TimeIn"].ToString();
                    item.TimeOut = dr["TimeOut"].ToString();
                    item.RenderedHours = dr["RenderedHours"].ToString();
                    item.LunchIn = dr["LunchIn"].ToString();
                    item.LunchOut = dr["LunchOut"].ToString();
                    item.TotalLunchHours = dr["TotalLunchHours"].ToString();
                    item.BreakInAm = dr["BreakInAm"].ToString();
                    item.BreakOutAm = dr["BreakOutAm"].ToString();
                    item.TotalBreakAmHours = dr["TotalBreakAmHours"].ToString();
                    item.BreakInPm = dr["BreakInPm"].ToString();
                    item.BreakOutPm = dr["BreakOutPm"].ToString();
                    item.Username = dr["Username"].ToString();
                    item.Fname = dr["Fname"].ToString();
                    item.Lname = dr["Lname"].ToString();
                    item.Mname = dr["Mname"].ToString();
                    item.Suffix = dr["Suffix"].ToString();
                    item.Email = dr["Email"].ToString();
                    item.FilePath = dr["FilePath"].ToString();
                    //item.UsertypeId = dr["UserType"].ToString();
                    item.StatusName = dr["StatusName"].ToString();
                    item.Task = dr["Task"].ToString();
                    item.EmployeeID = dr["EmployeeID"].ToString();
                    item.EmployeeType = dr["EmployeeType"].ToString();
                    item.TimelogStatus = dr["TimelogsStatus"].ToString();
                    item.IsUnderTime = dr["IsUnderTime"].ToString();
                    //item.DateCreated = Convert.ToDateTime(dr["DateCreated"].ToString()).ToString("MM-dd-yyyy");


                    result.Add(item);
                }


                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
