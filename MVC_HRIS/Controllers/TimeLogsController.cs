
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
using static MVC_HRIS.Controllers.OverTimeController;
using static MVC_HRIS.Controllers.FilingController;
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
        public IActionResult TLApproval()
        {
            return PartialView("TLApproval");
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
            public int? status { get; set; }
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
        //[HttpPost]
        //public async Task<IActionResult> TimeOut(User data)
        //{
        //    string res = "";
        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        var url = DBConn.HttpString + "/TimeLogs/TimeOut";

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        //        using (var response = await client.PostAsync(url, content))
        //        {
        //            HttpStatusCode statusCode = response.StatusCode;
        //            int numericStatusCode = (int)statusCode;
        //            if (numericStatusCode == 200)
        //            {
        //                res = numericStatusCode.ToString();
        //            }
        //            else
        //            {
        //                res = await response.Content.ReadAsStringAsync();
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        status = ex.GetBaseException().ToString();
        //    }
        //    return Json(new { status = res });
        //}

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
            public string? UnfiledOvertime { get; set; }
            public string? ApprovedOvertimeHours { get; set; }
            public string? UndertimeHours { get; set; }
            public string? ApprovedOffsetTimeHours { get; set; }
            public string? TotalHoursBySchedule { get; set; }
            public string? TotalHours { get; set; }
            public string? RequiredHours { get; set; }
            public string? DaysLate { get; set; }
            public string? WorkingDays { get; set; }
        }
        public class SummaryTimeLogsParam
        {
            public string? Usertype { get; set; }
            public string[]? UserId { get; set; }

            public string? datefrom { get; set; }
            public string? dateto { get; set; }
            public string? Department { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetSummaryTimelogsListManager(SummaryTimeLogsParam data)
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
        public class SummaryTimeLogsParamSelect
        {
            public string? fullname { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetSummaryTimelogsListSelect(SummaryTimeLogsParamSelect data)
        {
            string result = "";
            var list = new List<SummaryTimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                //var url = DBConn.HttpString + "/TimeLogs/TimeLogsListManager";
                var url = DBConn.HttpString + "/TimeLogs/SummaryTimeLogUserList";
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
        
        public async Task<IActionResult> ExportSummaryTimelogsList(SummaryTimeLogsParam data)
        {
            try
            {
                string sql = $@"DECLARE @StartDate DATE = '" + data.datefrom + "', @EndDate DATE = '" + data.dateto + "'";
                sql += $@" SELECT
							um.id as 'UserID'
							,um.Fullname
							,CASE
								WHEN COALESCE(SUM(TotalTime.RenderedHours),0)-(COALESCE(wd.WorkingDays,0)*8) < 0 
									THEN 0
								ELSE COALESCE(SUM(TotalTime.RenderedHours),0)-(COALESCE(wd.WorkingDays,0)*8)
							END AS 'UnfiledOvertime'
							,COALESCE(SUM(OverTime.HoursApproved),0)AS 'ApprovedOvertimeHours'
							,COALESCE(SUM(OffsetTime.HoursApproved),0)AS 'ApprovedOffsetTimeHours'
							,(COALESCE(wd.WorkingDays,0)*8)-(COALESCE(ComputedTotalHours.ApprovedRenderedHours,0)+COALESCE(SUM(OffsetTime.HoursApproved),0)) AS 'UndertimeHours'
							,COALESCE(ComputedTotalHours.ApprovedRenderedHours,0) AS 'TotalHoursBySchedule'
							,COALESCE(SUM(TotalTime.RenderedHours),0) AS 'TotalHours'
							,COALESCE(lc.LateCount,0) AS 'DaysLate'
							,COALESCE(wd.WorkingDays,0) AS 'WorkingDays'
							,COALESCE(wd.WorkingDays,0)*8 AS 'RequiredHours'
						FROM tbl_UsersModel um WITH(NOLOCK)

						LEFT JOIN (SELECT SUM(RenderedHours) AS RenderedHours, UserId FROM tbl_TimeLogs WITH(NOLOCK) WHERE StatusId = '1' AND Date BETWEEN @StartDate AND @EndDate GROUP BY UserId)TotalTime 
						ON um.ID = TotalTime.UserId
						LEFT JOIN (SELECT SUM(HoursApproved) AS HoursApproved,EmployeeNo FROM TblOvertimeModel WHERE Status = '5' AND ConvertToLeave = 0 AND ConvertToOffset = 0 AND Date BETWEEN @StartDate AND @EndDate GROUP BY EmployeeNo  )OverTime 
						ON um.EmployeeID = OverTime.EmployeeNo
						LEFT JOIN (SELECT SUM(HoursApproved) AS HoursApproved,EmployeeNo FROM TblOvertimeModel WHERE Status = '5' AND ConvertToLeave = 0 AND ConvertToOffset = 1 AND Date BETWEEN @StartDate AND @EndDate GROUP BY EmployeeNo  )OffsetTime 
						ON um.EmployeeID = OffsetTime.EmployeeNo
						LEFT JOIN (SELECT et.Id AS ETypeID ,SUM(WD.WORKINGDAYS) AS WorkingDays FROM tbl_EmployeeType et WITH(NOLOCK)
									INNER JOIN TblScheduleDayModels sd WITH(NOLOCK)
									ON et.ScheduleId = sd.SchceduleId 
									LEFT JOIN (SELECT DISTINCT 
										DATEADD(DAY, n, @StartDate) AS DateValue
										,(DATEPART(WEEKDAY, DATEADD(DAY, n, @StartDate)) + @@DATEFIRST - 2) % 7 AS WorkingDayName
										,CASE
											WHEN SD.STARTTIME IS NOT NULL THEN 1
											ELSE 0
										END AS WorkingDays
									FROM (
										SELECT TOP (DATEDIFF(DAY, @StartDate, @EndDate) + 1) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
										FROM master.dbo.spt_values
									) AS Tally
									LEFT JOIN TblScheduleDayModels sd WITH(NOLOCK)
									ON sd.day = (DATEPART(WEEKDAY, DATEADD(DAY, n, @StartDate)) + @@DATEFIRST - 2) % 7) AS wd
									on wd.WorkingDayName = sd.day
									GROUP BY et.Id) wd
									ON WD.ETypeID = UM.EmployeeType
							LEFT JOIN (SELECT 
										um.Id                       
										,SUM(Case
											when sd.starttime is null then 0
											when Right(tl.TimeIn,5) > sd.starttime then 1
											else 0
										End ) as LateCount
	                       
									FROM tbl_TimeLogs tl WITH(NOLOCK)
									LEFT JOIN tbl_UsersModel um WITH(NOLOCK)
									ON tl.UserId = um.ID
									LEFT JOIN tbl_EmployeeType et WITH(NOLOCK)
									ON et.ID = um.EmployeeType
									LEFT JOIN TblScheduleDayModels sd WITH(NOLOCK)
									ON et.ScheduleId = sd.SchceduleId and sd.day = (DATEPART(WEEKDAY, tl.date) + @@DATEFIRST - 2) % 7
									LEFT JOIN tbl_TimeLogStatus ts WITH(NOLOCK)
									ON ts.StatusId = tl.StatusId
									LEFT JOIN tbl_TaskModel t WITH(NOLOCK)
									ON t.Id = tl.TaskId
									WHERE tl.Date between @StartDate AND @EndDate and UserId = 3059 
									GROUP BY um.Id) lc
									ON lc.Id = um.Id
							LEFT JOIN (SELECT 
										um.Id as UserId
										,SUM(CASE
											WHEN sd.starttime IS NULL 
												THEN DATEDIFF(MINUTE, CONVERT(datetime,REPLACE(tl.TimeIn, 'T', ' ')), CONVERT(datetime, REPLACE(tl.TimeOut, 'T', ' ')))/60.0
											ELSE
											CASE
												WHEN REPLACE(tl.TimeOut, 'T', ' ')>CONCAT(LEFT(tl.TimeOut, 10),' ',sd.endtime)
													THEN 
														CASE
															WHEN REPLACE(tl.TimeIn, 'T', ' ')<CONCAT(LEFT(tl.TimeIn, 10),' ',sd.starttime)
																THEN
																	DATEDIFF(MINUTE, CONVERT(datetime, CONCAT(LEFT(tl.TimeIn, 10),' ',sd.starttime)), CONVERT(datetime, CONCAT(LEFT(tl.TimeOut, 10),' ',sd.endtime)))/60.0
																ELSE
																	DATEDIFF(MINUTE, CONVERT(datetime,REPLACE(tl.TimeIn, 'T', ' ')), CONVERT(datetime, CONCAT(LEFT(tl.TimeOut, 10),' ',sd.endtime)))/60.0
														END
				
													ELSE 
														CASE
															WHEN REPLACE(tl.TimeIn, 'T', ' ')<CONCAT(LEFT(tl.TimeIn, 10),' ',sd.starttime)
																THEN
																	--0--DATEDIFF(MINUTE, CONVERT(datetime, CONCAT(LEFT(tl.TimeIn, 10),' ',sd.starttime)), CONVERT(datetime, CONCAT(LEFT(tl.TimeOut, 10),' ',sd.endtime)))/60.0
																	DATEDIFF(MINUTE, CONVERT(datetime,CONCAT(LEFT(tl.TimeIn, 10),' ',sd.starttime)), CONVERT(datetime, REPLACE(tl.TimeOut, 'T', ' ')))/60.0
																ELSE
																	DATEDIFF(MINUTE, CONVERT(datetime,REPLACE(tl.TimeIn, 'T', ' ')), CONVERT(datetime, REPLACE(tl.TimeOut, 'T', ' ')))/60.0
														END	
											END
										END) AS ApprovedRenderedHours
	
									FROM tbl_TimeLogs tl WITH(NOLOCK)
									LEFT JOIN tbl_UsersModel um WITH(NOLOCK)
									ON tl.UserId = um.ID
									LEFT JOIN tbl_EmployeeType et WITH(NOLOCK)
									ON et.ID = um.EmployeeType
									LEFT JOIN TblScheduleDayModels sd WITH(NOLOCK)
									ON et.ScheduleId = sd.SchceduleId and sd.day = (DATEPART(WEEKDAY, tl.date) + @@DATEFIRST - 2) % 7
									LEFT JOIN tbl_TimeLogStatus ts WITH(NOLOCK)
									ON ts.StatusId = tl.StatusId
									LEFT JOIN tbl_TaskModel t WITH(NOLOCK)
									ON t.Id = tl.TaskId
									WHERE tl.Date between @StartDate AND @EndDate and tl.StatusId = 1 
									GROUP BY um.Id ,tl.StatusId) ComputedTotalHours
									ON ComputedTotalHours.UserId = um.Id
                                WHERE um.Active = 1";


                if (data.Department != "0")
                {
                    sql += " AND um.Department = '" + data.Department + "'";
                }
                if (data.UserId != null)
                {
                    sql += " AND um.Id in (";
                    for (int x = 0; x < data.UserId.Length; x++)
                    {
                        sql += data.UserId[x] + ',';
                    }
                    sql += "'0')";
                }

                sql += "  GROUP BY um.Fullname, um.id, wd.WorkingDays, lc.LateCount, ComputedTotalHours.ApprovedRenderedHours";
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
        public class CheckedTLRequestListParam
        {
            public string[]? Id { get; set; }
        }
        public partial class TblTimelogsVM
        {
            public string? Id { get; set; }
            public string? fullname { get; set; }
            public string? Remarks { get; set; }
            public string? TimeIn { get; set; }
            public string? TimeOut { get; set; }
            public string? RenderedHours { get; set; }
            public string? ApprovalReason { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetCheckedTLRequestList(CheckedTLRequestListParam data)
        {
            string result = "";
            var list = new List<TblTimelogsVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeLogs/CheckedTLRequestList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblTimelogsVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }

        public class multiApprovalParamTLList
        {
            public int? Id { get; set; }
            public string? reason { get; set; }
        }
        public class multiApprovalTLParam
        {
            public int? Status { get; set; }
            public List<multiApprovalParamTLList>? tlapproval { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> MultiApproval(multiApprovalTLParam data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Timelogs/MultiUpdateStatus";

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

        public partial class TblTimelogsImportModel
        {
            public string? UserId { get; set; }
            public string? Date { get; set; }
            public string? TimeIn { get; set; }
            public string? TimeOut { get; set; }
            public string? TaskId { get; set; }
            public string? Remarks { get; set; }
            public string? TotalLunchHours { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> TLIndex(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            System.Text.Encoding.RegisterProvider(
            System.Text.CodePagesEncodingProvider.Instance);
            try
            {
                if (file == null)
                {
                    ViewData["Message"] = "Error: Please select a file.";
                }
                else
                {
                    if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx"))
                    {
                        ViewData["Message"] = "Error: Invalid file.";
                        string filename = $"{hostingEnvironment.WebRootPath}\\excel\\{file.FileName}";
                        using (FileStream fileStream = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fileStream);
                            fileStream.Flush();
                        }

                        IExcelDataReader reader = null;
                        FileStream stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read);

                        if (file.FileName.EndsWith("xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        if (file.FileName.EndsWith("xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        int i = 0;

                        var data = new List<TblTimelogsImportModel>();

                        while (reader.Read())
                        {
                            i++;

                            if (i > 1) // Skipping header row
                            {
                                // Check if at least one column has data
                                if (string.IsNullOrWhiteSpace(reader.GetValue(0)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(1)?.ToString())
                                    )
                                {
                                    continue; // Skip this row
                                }

                                // Process the row
                                data.Add(new TblTimelogsImportModel
                                {

                                    UserId = reader.GetValue(12)?.ToString() ?? "",
                                    TimeIn = reader.GetValue(3)?.ToString() ?? "",
                                    TimeOut = reader.GetValue(6)?.ToString() ?? "",
                                    TaskId = reader.GetValue(9)?.ToString() ?? "",
                                    Remarks = reader.GetValue(10)?.ToString() ?? "",
                                    Date = reader.GetValue(0)?.ToString() ?? "",
                                    TotalLunchHours = reader.GetValue(7)?.ToString() ?? ""
                                });
                            }
                        }
                        reader.Close();
                        System.IO.File.Delete(filename);

                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/TimeLogs/Import";

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                        StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        using (var response = await client.PostAsync(url, content))
                        {
                            status = await response.Content.ReadAsStringAsync();
                        }

                        ViewData["Message"] = "New Entry" + status;
                    }
                    else
                    {
                        ViewData["Message"] = "Error: Invalid file.";
                    }
                }
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
            return View("Index");
        }

        public IActionResult DownloadHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Shift Date";
                ws.Cells["B1"].Value = "Start Date";
                ws.Cells["C1"].Value = "Start Time";
                ws.Cells["D1"].Value = "Time In";
                ws.Cells["E1"].Value = "End Date";
                ws.Cells["F1"].Value = "End Time";
                ws.Cells["G1"].Value = "Time Out";
                ws.Cells["H1"].Value = "Break (Hour/s)";
                ws.Cells["I1"].Value = "Task";
                ws.Cells["J1"].Value = "TaskId";
                ws.Cells["K1"].Value = "Task Description";

                ws.Cells["L1"].Value = "Employee No.";
                ws.Cells["M1"].Value = "UserID";


                // Apply Custom Formatting
                ws.Cells["A:A"].Style.Numberformat.Format = "yyyy-mm-dd"; // Date format with microseconds
                ws.Cells["B:B"].Style.Numberformat.Format = "@";
                ws.Cells["C:C"].Style.Numberformat.Format = "@";
                ws.Cells["E:E"].Style.Numberformat.Format = "@";
                ws.Cells["F:F"].Style.Numberformat.Format = "@";
                ws.Cells["H:H"].Style.Numberformat.Format = "0.00";

                ws.Cells["A2"].Value = "yyyy-mm-dd";
                ws.Cells["B2"].Value = "yyyy-mm-dd";
                ws.Cells["C2"].Value = "hh:mm:ss";
                ws.Cells["E2"].Value = "yyyy-mm-dd";
                ws.Cells["F2"].Value = "hh:mm:ss";
                ws.Cells["H2"].Value = "0.00";

                ws.Cells["L2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["M2"].Value = HttpContext.Session.GetString("Id");
                // Enforce Boolean (Dropdown with TRUE/FALSE)
                for (var col = 1; col <= 12; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }




                ws.Cells["Q1:Q2"].Style.Font.Italic = true;
                ws.Cells["Q1:Q2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["Q1"].Value = "All Fields are required";
                ws.Cells["Q2"].Value = "Please follow the format shown in the first row.";
                string sqlTask = $@"SELECT [Id], [Title] FROM [ODC_HRIS].[dbo].[tbl_TaskModel] WHERE isBreak = 0";
                var dataSet = db.SelectDb(sqlTask);

                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }

                DataTable dt = dataSet.Tables[0];
                int ctr = 2;
                var validation = ws.DataValidations.AddListValidation("I2:I1000");
                foreach (DataRow dr in dt.Rows)
                {
                    ws.Cells["R" + ctr].Value = dr["Title"].ToString();
                    ws.Cells["S" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    validation.Formula.Values.Add(dr["Title"].ToString());
                    ctr++;
                }
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = "Invalid Selection";
                validation.Error = "Please select a valid option from the dropdown list.";
                int ctrTierId = 50;
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["J" + i].Formula = "=IFERROR(VLOOKUP(I" + i + ",R2:S10,2,FALSE),\"\")";
                }
                for (int row = 2; row <= 50; row++)
                {
                    ws.Cells[$"D{row}"].Formula = $"=IF(B{row}=\"\",\"\",CONCAT(B{row},\"T\",C{row}))";
                    ws.Cells[$"G{row}"].Formula = $"=IF(E{row}=\"\",\"\",CONCAT(E{row},\"T\",F{row}))";
                }
                ws.Cells.AutoFitColumns();
                //int[] lockedColumns = { 4, 10, 11, 12, 13, 14, 15, 16, 17 };

                int[] hideColumns = { 4, 7, 10, 12, 13, 14, 15, 16, 18, 19 };
                //foreach (int col in lockedColumns)
                //{
                //    ws.Column(col).Style.Locked = true; // Lock column
                //}
                foreach (int col in hideColumns)
                {
                    //ws.Column(col).Hidden = true;       // Hide column
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                ws.Cells["A:C"].Style.Locked = false;
                ws.Cells["E:F"].Style.Locked = false;
                ws.Cells["H:I"].Style.Locked = false;
                ws.Cells["K:K"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock

                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Request-Timelog-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}