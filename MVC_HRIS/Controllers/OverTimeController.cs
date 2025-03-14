
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
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace MVC_HRIS.Controllers
{
    public class OverTimeController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public OverTimeController(IOptions<AppSettings> appSettings, QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }
        public class EmployeeIdFilter
        {
            public string EmployeeNo { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetOverTimeList(EmployeeIdFilter data)
        {
            string result = "";
            var list = new List<TblOvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/OvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblOvertimeVM>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        public IActionResult OverTime()
        {
            return View("_OverTime");
        }
        public IActionResult OTFiling()
        {

            return PartialView("OTFiling");
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
        public async Task<IActionResult> SaveOT(TblOvertimeModelView data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/save";

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

        public partial class OvertimeVM
        {
            public int Id { get; set; }
            public string OTNo { get; set; }
            public string EmployeeNo { get; set; }
            public string fullname { get; set; }
            public DateTime? Date { get; set; }
            [DataType(DataType.Date)]
            public TimeSpan? StartTime { get; set; }
            [DataType(DataType.Time)]
            public TimeSpan? EndTime { get; set; }
            [DataType(DataType.Time)]
            public DateTime? StartDate { get; set; }
            [DataType(DataType.Date)]
            public DateTime? EndDate { get; set; }
            public decimal? HoursFiled { get; set; }
            public decimal? HoursApproved { get; set; }
            public string? Remarks { get; set; }
            public bool? ConvertToLeave { get; set; }
            public DateTime? DateCreated { get; set; }
            public int? LeaveId { get; set; }
            public int? Status { get; set; }
            public string? StatusName { get; set; }
            public int? CreatedBy { get; set; }
            public bool? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public DateTime? DateDeleted { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingOvertTimeListSelect(EmployeeIdFilter data)
        {
            string result = "";
            var list = new List<OvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/OverTime/PendingOvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<OvertimeVM>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingOvertTimeList(EmployeeIdFilter data)
        {
            string result = "";
            var list = new List<OvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/OverTime/PendingOvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<OvertimeVM>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }

        public class OTUpdateStatus
        {
            public int Id { get; set; }
            public int status { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> updateStatus(OTUpdateStatus data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/updateStatus";

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
    }
}
