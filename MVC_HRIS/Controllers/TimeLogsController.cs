
using MVC_HRIS.Models;
using CMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Data;


using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MVC_HRIS.Services;
using System.Text;
using System;
using AuthSystem.Manager;
using MVC_HRIS.Manager;
using ExcelDataReader;
using System.Collections.Generic;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.ComponentModel;
using System.Drawing;
using Net.SourceForge.Koogra.Excel2007.OX;
using MVC_HRIS.Models;
using MVC_HRIS.Services;
using API_HRIS.Manager;
using Microsoft.EntityFrameworkCore;
using MVC_HRIS.Models;
using System.Net;
using OfficeOpenXml.DataValidation;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Net.SourceForge.Koogra.Excel2007.OX;
using ExcelDataReader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using API_HRIS.Models;
namespace MVC_HRIS.Controllers
{
    public class TimeLogsController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public TimeLogsController(IOptions<AppSettings> appSettings, QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {//update
            string token = HttpContext.Session.GetString("Bearer");
            if (token == "")
            {
                return RedirectToAction("Index", "LogIn");
            }
            return View();
        }
        public IActionResult ManagerIndex()
        {//update
            string token = HttpContext.Session.GetString("Bearer");
            if (token == "")
            {
                return RedirectToAction("Index", "LogIn");
            }
            return View();
        }
        public IActionResult ManagerNotification()
        {//update
            string token = HttpContext.Session.GetString("Bearer");
            if (token == "")
            {
                return RedirectToAction("Index", "LogIn");
            }
            return View();
        }
        public class TimeLogsParam
        {
            public string? Usertype { get; set; }
            public string? UserId { get; set; }

            public string? datefrom { get; set; }
            public string? dateto { get; set; }
            public string? Department { get; set; }
        }
        public partial class User
        {
            public int? UserId { get; set; }

            //

        }
        public class CheckBreakTimeParameter
        {
            public int userId { get; set; }
            public string? TimeOfDay { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> PostCheckBreakTime(CheckBreakTimeParameter data)
        {
            string result = "";
            var list = new List<TimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/CheckBreakTime";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);

                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }

            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> GetLastTimeIn(User data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/getLastTimeIn";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    res = await response.Content.ReadAsStringAsync();

                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        [HttpPost]
        public async Task<IActionResult> TimeOut(User data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeOut";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        [HttpPost]
        public async Task<IActionResult> RegularTimeOut(User data)
        {
            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/RegularTimeOut";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        [HttpPost]
        public async Task<IActionResult> GetTimelogsList(TimeLogsParam data)
        {
            string result = "";
            var list = new List<TimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        [HttpPost]
        public async Task<IActionResult> GetPedingTimelogsList(TimeLogsParam data)
        {
            string result = "";
            var list = new List<TimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsPending";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        [HttpPost]
        public async Task<IActionResult> GetTimelogsCount(TimeLogsParam data)
        {
            string result = "";
            var list = new List<TimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> GetTimelogsTotalHours(TimeLogsParam data)
        {
            string result = "";
            var list = new List<TimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(list);
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
        public async Task<IActionResult> GetTimelogsListManager(TimeLogsParam data)
        {
            string result = "";
            var list = new List<ManagerTimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                //var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                var url = DBConn.HttpString + "/TimeLogs/RegularTimeLogList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<ManagerTimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        public class SummaryTimelogsVM
        {
            public string? UserID { get; set; }
            public string? Fullname { get; set; }
            public string? ApprovedOvertimeHours { get; set; }
            public string? UndertimeHours { get; set; }
            public string? ApprovedOffsetTimeHours { get; set; }
            public string? ApprovedTotalHours { get; set; }
            public string? RequiredHours { get; set; }
            public string? DaysLate { get; set; }
            public string? WorkingDays { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetSummaryTimelogsListManager(TimeLogsParam data)
        {
            string result = "";
            var list = new List<SummaryTimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                //var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                var url = DBConn.HttpString + "/TimeLogs/SummaryTimeLogList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<SummaryTimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        [HttpPost]
        public async Task<IActionResult> GetSummaryTimelogsListSelect(TimeLogsParam data)
        {
            string result = "";
            var list = new List<SummaryTimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                //var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                var url = DBConn.HttpString + "/TimeLogs/SummaryTimeLogList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<SummaryTimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> GetNotificationList(TblNotification data)
        {
            string result = "";
            var list = new List<TblNotification>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/NotificationList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblNotification>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        [HttpGet]
        public async Task<IActionResult> GetNotificationPendingCount()
        {
            string result = "";
            try
            {
                string test = token_.GetValue();
                var url = DBConn.HttpString + "/TimeLogs/NotificationUnreadCount";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

                string response = await client.GetStringAsync(url);
                result = response;


            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(result);
        }
        
        public async Task<IActionResult> ExportSummaryTimelogsList(TimeLogsParam data)
        {
            try
            {
                string sql = $@"DECLARE @StartDate DATE = '" + data.datefrom + "', @EndDate DATE = '" + data.dateto + "'";
                sql += $@" SELECT
	                        um.id as 'UserID'
	                        ,um.Fullname
	                        ,COALESCE(SUM(OverTime.HoursApproved),0)AS 'ApprovedOvertimeHours'
	                        ,CASE
		                        WHEN COALESCE(COALESCE(workingDays.WorkingDays*8, 0) - SUM(TotalTime.RenderedHours),0) < 0 THEN 0
		                        ELSE COALESCE(COALESCE(workingDays.WorkingDays*8, 0) - SUM(TotalTime.RenderedHours),0)
	                        END AS 'UndertimeHours'
	                        ,COALESCE(SUM(OffsetTime.HoursApproved),0)AS 'ApprovedOffsetTimeHours'
	                        ,COALESCE(SUM(TotalTime.RenderedHours),0) AS 'ApprovedTotalHours'
	                        ,COALESCE(workingDays.WorkingDays*8, 0) 'RequiredHours'
	                        ,COALESCE(SUM(late.LateCount), 0) 'DaysLate'
	                        ,COALESCE(workingDays.WorkingDays, 0) 'WorkingDays'
                        FROM tbl_UsersModel um WITH(NOLOCK)

                        LEFT JOIN (SELECT SUM(RenderedHours) AS RenderedHours, UserId FROM tbl_TimeLogs WITH(NOLOCK) WHERE StatusId = '1' AND Date BETWEEN @StartDate AND @EndDate GROUP BY UserId)TotalTime 
                        ON um.ID = TotalTime.UserId

                        LEFT JOIN (SELECT SUM(HoursApproved) AS HoursApproved,EmployeeNo FROM TblOvertimeModel WHERE Status = '5' AND ConvertToLeave = 0 AND ConvertToOffset = 0 AND Date BETWEEN @StartDate AND @EndDate GROUP BY EmployeeNo  )OverTime 
                        ON um.EmployeeID = OverTime.EmployeeNo
                        LEFT JOIN (SELECT SUM(HoursApproved) AS HoursApproved,EmployeeNo FROM TblOvertimeModel WHERE Status = '5' AND ConvertToLeave = 0 AND ConvertToOffset = 1 AND Date BETWEEN @StartDate AND @EndDate GROUP BY EmployeeNo  )OffsetTime 
                        ON um.EmployeeID = OffsetTime.EmployeeNo

                        LEFT JOIN (SELECT 
	                        WorkingDays.EmployeeTypeId
	                        ,COUNT(WorkingDays.Sched) 'WorkingDays'
                        FROM (SELECT 
                            DATEADD(DAY, number, @StartDate) AS DateValue,
                            DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate)) AS DayName,
	                        CASE
		                        WHEN mon.Monday IS NOT NULL THEN mon.Id
		                        WHEN tues.Tuesday IS NOT NULL THEN tues.Id
		                        WHEN wed.Wednesday IS NOT NULL THEN wed.Id
		                        WHEN thurs.Thursday IS NOT NULL THEN thurs.Id
		                        WHEN fri.Friday IS NOT NULL THEN fri.Id
		                        WHEN sat.Saturday IS NOT NULL THEN sat.Id
		                        WHEN sun.Sunday IS NOT NULL THEN sun.Id
		                        ELSE 0
	                        END AS 'EmployeeTypeId'
	                        ,CASE
		                        WHEN mon.Monday IS NOT NULL THEN 1 
		                        WHEN tues.Tuesday IS NOT NULL THEN 1
		                        WHEN wed.Wednesday IS NOT NULL THEN 1
		                        WHEN thurs.Thursday IS NOT NULL THEN 1
		                        WHEN fri.Friday IS NOT NULL THEN 1
		                        WHEN sat.Saturday IS NOT NULL THEN 1
		                        WHEN sun.Sunday IS NOT NULL THEN 1
		                        ELSE 0
	                        END AS 'Sched'
                        FROM master.dbo.spt_values AS date

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.MondayS IS NOT NULL THEN 'Monday'
	                        END AS 'Monday'

                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) mon
                        ON mon.Monday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.TuesdayS IS NOT NULL THEN 'Tuesday'
	                        END AS 'Tuesday'
                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) tues
                        ON tues.Tuesday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.WednesdayS IS NOT NULL THEN 'Wednesday'
	                        END AS 'Wednesday'
                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) wed
                        ON wed.Wednesday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.ThursdayS IS NOT NULL THEN 'Thursday'
	                        END AS 'Thursday'
                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) thurs
                        ON thurs.Thursday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.FridayS IS NOT NULL THEN 'Friday'
	                        END AS 'Friday'
                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) fri
                        ON fri.Friday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.SaturdayS IS NOT NULL THEN 'Saturday'
	                        END AS 'Saturday'
                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) sat
                        ON sat.Saturday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        LEFT JOIN (SELECT 
	                        et.Id,
	                        CASE
		                        WHEN sched.SundayS IS NOT NULL THEN 'Sunday'
	                        END AS 'Sunday'
                        FROM tbl_EmployeeType et WITH(NOLOCK)
                        INNER JOIN tbl_ScheduleModel sched WITH(NOLOCK)
                        ON sched.Id = et.ScheduleId) sun
                        ON sun.Sunday = DATENAME(WEEKDAY, DATEADD(DAY, date.number, @StartDate))

                        WHERE date.type = 'P' 
                        AND number BETWEEN 0 AND DATEDIFF(DAY, @StartDate, @EndDate))WorkingDays
                        GROUP BY WorkingDays.EmployeeTypeId) workingDays
                        on um.EmployeeType = workingDays.EmployeeTypeId

                        LEFT JOIN (
                        SELECT
	                        lateCount.userId
	                        ,COUNT(lateCount.TimeLogsStatus) LateCount
                        FROM
                        (SELECT 
	                        um.id AS 'userId'
	                        ,tl.Date
	                        ,SUM(CASE
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Monday' 
		                        THEN 
			                        CASE
				                        WHEN sched.MondayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.MondayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Tuesday' 
		                        THEN 
			                        CASE
				                        WHEN sched.TuesdayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.TuesdayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Wednesday' 
		                        THEN 
			                        CASE
				                        WHEN sched.WednesdayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.WednesdayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Thursday' 
		                        THEN 
			                        CASE
				                        WHEN sched.ThursdayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.ThursdayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Friday' 
		                        THEN 
			                        CASE
				                        WHEN sched.FridayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.FridayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Saturday' 
		                        THEN 
			                        CASE
				                        WHEN sched.SaturdayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.SaturdayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        WHEN DATENAME(WEEKDAY, LEFT(tl.TimeIn,10)) = 'Sunday' 
		                        THEN 
			                        CASE
				                        WHEN sched.SundayS IS NULL THEN 0
				                        ELSE
				
				                        CASE
					                        WHEN RIGHT(tl.TimeIn, 5) > sched.SundayS
					                        THEN 1
					                        ELSE 0
				                        END
			                        END
		                        ELSE NULL
	                        END )as TimeLogsStatus
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
                        WHERE tl.Date between @StartDate AND @EndDate 
                        GROUP BY um.id, sched.MondayS, sched.TuesdayS, sched.WednesdayS, sched.ThursdayS, sched.FridayS, sched.SaturdayS, sched.SundayS, sched.MondayE, sched.TuesdayE, sched.WednesdayE, sched.ThursdayE, sched.FridayE, sched.SaturdayE, sched.SundayE,tl.Date

                        ) lateCount

                        WHERE lateCount.TimeLogsStatus != 0 
                        GROUP BY lateCount.userId) AS late
                        ON late.userId = um.ID
                WHERE um.Id IS NOT NULL";


                if (data.Department != "0")
                {
                    sql += " AND um.Department = '" + data.Department + "'";
                }
                if (data.UserId != "0")
                {
                    sql += " AND um.Id = '" + data.UserId + "'";
                }

                sql += "  GROUP BY um.Fullname, workingDays.WorkingDays, um.id";
                string stm = sql;
                DataSet ds = db.SelectDb(sql);
                var stream = new MemoryStream();

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    worksheet.Cells["A:AZ"].Style.Font.Size = 11;

                    worksheet.Cells["A1"].Value = "Employee Timelogs Report";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.SetFromFont(new System.Drawing.Font("Arial Black", 22));
                    worksheet.Cells["A3"].Value = "Date Printed:     " + DateTime.Now.ToString("yyyy-MM-dd"); ;

                    // Format the "Date" column (assuming it's in column E, adjust if necessary)
                    //worksheet.Column(5).Style.Numberformat.Format = "yyyy-MM-dd"; // Column 5 corresponds
                    worksheet.Cells["A6:Z6"].Style.Font.Bold = true;
                    worksheet.Cells["A6"].LoadFromDataTable(ds.Tables[0], true);
                    worksheet.Cells["A6:Z10000"].AutoFitColumns();

                    package.Save();
                }
                stream.Position = 0;
                string excelName = "summarytimelogs.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error exporting timelogs: {ex.Message}");
                return StatusCode(500, "An error occurred while generating the Excel file.");
            }
        }
        [HttpPost]
        public async Task<List<TimelogsVM>> ExportTimelogsListManager(TimeLogsParam data)
        {
            var list = new List<TimelogsVM>();
            try
            {
                // Fetch the data from the external API
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);
                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
                // Handle exception (e.g., log it or return an error response)
            }

            return list;
        }
        //public async Task<IActionResult> DLExportTimelogsListManager(TimeLogsParam data)
        //{
        //    var listing = await ExportTimelogsListManager(data);

        //    // You don't need to call ToList() here because ExportTimelogsListManager already returns a List<TimelogsVM>
        //    var list = listing;  // This is already a List<TimelogsVM>
        //    //var list = new List<TimelogsVM>();
        //    //var list = listing.ToList();
        //    try
        //    {

        //        var stream = new MemoryStream();
        //        using (var pck = new ExcelPackage(stream))
        //        {
        //            // Create a worksheet
        //            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");

        //            // Define headers
        //            ws.Cells[1, 1].Value = "UserId";           // Example column
        //            ws.Cells[1, 2].Value = "EmployeeID";     // Example column
        //            ws.Cells[1, 3].Value = "Date";         // Example column
        //            ws.Cells[1, 4].Value = "Hours";        // Example column
        //            ws.Cells[1, 5].Value = "Description";  // Example column

        //            // Populate data
        //            int row = 2;
        //            foreach (var log in list)
        //            {
        //                ws.Cells[row, 1].Value = log.UserId;          // Replace with actual property name
        //                ws.Cells[row, 2].Value = log.EmployeeID;    // Replace with actual property name
        //                ws.Cells[row, 3].Value = log.Date;        // Replace with actual property name
        //                ws.Cells[row, 4].Value = log.RenderedHours;       // Replace with actual property name
        //                ws.Cells[row, 5].Value = log.Remarks; // Replace with actual property name
        //                row++;
        //            }
        //            // Auto-fit columns
        //            ws.Cells[ws.Dimension.Address].AutoFitColumns();

        //            // Save to the memory stream
        //            pck.SaveAs(stream);

        //        }
        //        stream.Position = 0;
        //        //string excelName = "" + HttpContext.Session.GetString("CorporateName") + "-AOPC-Call to Action Result.xlsx";
        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TimeLogs.xlsx");
        //    }
        //    catch (Exception ex)
        //    {
        //        string status = ex.GetBaseException().ToString();
        //        // Handle exception (e.g., log it or return an error response)
        //    }

        //    // Return JSON data if something goes wrong
        //    return Json(list);
        //}
        public async Task<IActionResult> DLExportTimelogsListManager(TimeLogsParam data)
        {
            try
            {

                string sql = $@"select 
                        u.fullname as 'Name',
                        u.username as 'Username',
                        t.Title as 'Task Title',
                        tl.remarks as 'Task Description',
                        tl.Date as 'Date',
                        tl.timein as 'Timein',
                        tl.timeout as 'Timeout',
                        tl.RenderedHours as 'Rendered Hours'
                    from tbl_timelogs tl with(nolock)
                    left join tbl_usersmodel u with(nolock)
                    on u.id = tl.userid
                    left join tbl_TaskModel t with(nolock)
                    on tl.TaskId = t.id
                    where tl.DeleteFlag <> 0 and tl.Date between '" + data.datefrom + "' and '" + data.dateto + "'";
                if (data.UserId != "0")
                {
                    sql += " and u.id = " + data.UserId;
                }
                if (data.Department != "0")
                {
                    sql += " and u.Department = " + data.Department;
                }

                string stm = sql;
                DataSet ds = db.SelectDb(sql);
                var stream = new MemoryStream();

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    worksheet.Cells["A:AZ"].Style.Font.Size = 11;

                    worksheet.Cells["A1"].Value = "Employee Timelogs Report";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.SetFromFont(new System.Drawing.Font("Arial Black", 22));
                    worksheet.Cells["A3"].Value = "Date Printed:     " + DateTime.Now.ToString("yyyy-MM-dd"); ;

                    // Format the "Date" column (assuming it's in column E, adjust if necessary)
                    worksheet.Column(5).Style.Numberformat.Format = "yyyy-MM-dd"; // Column 5 corresponds
                    worksheet.Cells["A6:Z6"].Style.Font.Bold = true;
                    worksheet.Cells["A6"].LoadFromDataTable(ds.Tables[0], true);
                    worksheet.Cells["A6:Z10000"].AutoFitColumns();

                    package.Save();
                }
                stream.Position = 0;
                string excelName = "timelogs.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error exporting timelogs: {ex.Message}");
                return StatusCode(500, "An error occurred while generating the Excel file.");
            }
        }
        public async Task<IActionResult> GetTimelogsListSelect(TimeLogsParam data)
        {
            string result = "";
            var list = new List<TimelogsVM>();
            try
            {//
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }


        public async Task<IActionResult> GetPendingTimelogsListSelect(TimeLogsParam data)
        {
            string result = "";
            var list = new List<ManagerTimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeLogsPending";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<ManagerTimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> TimeIn(TblTimeLog data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/TimeIn";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        [HttpPost]
        public async Task<IActionResult> RegularTimeIn(TblTimeLog data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/RegularTimeIn";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        public partial class BreakTimeParam
        {
            public int? UserId { get; set; }
            public int? Meridiem { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> RegularBreak(BreakTimeParam data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/BreakTime";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }

        [HttpPost]
        public async Task<IActionResult> ManualLogs(TblTimeLog data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/ManualLogs";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        public class TimeLogId
        {
            public int Id { get; set; }
            public int Action { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLogStatus(TimeLogId data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/UpdateLogStatus";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        [HttpPost]
        public async Task<IActionResult> LogsNotification(TblNotification data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/LogsNotification";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        res = numericStatusCode.ToString();
                    }
                    else
                    {
                        res = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        public IActionResult TaskModal()
        {

            return PartialView("TaskModal");
        }
    }
}