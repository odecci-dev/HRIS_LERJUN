
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
using API_HRIS.Models;
using System.Net;

namespace MVC_HRIS.Controllers
{
    public class DashboardController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public DashboardController(IOptions<AppSettings> appSettings,  QueryValueService _token,
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
            if (token == "" || token == null)
            {
                return RedirectToAction("Index", "LogIn");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetEmploymentTypeList()
        {
            string result = "";

            var list = new List<TblEmployeeTypeModel>();
            try
            {
                string test = token_.GetValue();
                var url = DBConn.HttpString + "/EmployeeType/eTypeList";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                string response = await client.GetStringAsync(url);
                result = response;

                list = JsonConvert.DeserializeObject<List<TblEmployeeTypeModel>>(result);
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
           
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeTypeCount()
        {
            string result = "";

            var list = new List<UserTypeCountModel>();
            try
            {
                string test = token_.GetValue();
                var url = DBConn.HttpString + "/Employee/GetEmployeeTypeCount";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                string response = await client.GetStringAsync(url);
                result = response;

                list = JsonConvert.DeserializeObject<List<UserTypeCountModel>>(result);
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetScheduleList()
        {
            try
            {
                string token = token_.GetValue();
                var url = DBConn.HttpString + "/TimeScheduler/GetSchedules";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

                string response = await client.GetStringAsync(url);
                List<TblScheduleModelVM> schedules = JsonConvert.DeserializeObject<List<TblScheduleModelVM>>(response);
                var events = schedules.Select(s => new
                {
                    id = s.id,
                    title = $"Shift: {s.shiftStartTime} - {s.shiftEndTime}",
                    start = s.scheduleDate.ToString("yyyy-MM-dd") + "T" + s.shiftStartTime,
                    end = s.scheduleDate.ToString("yyyy-MM-dd") + "T" + s.shiftEndTime,
                    start_break = s.scheduleDate.ToString("yyyy-MM-dd") + "T" + s.breakStartTime,
                    end_break = s.scheduleDate.ToString("yyyy-MM-dd") + "T" + s.breakEndTime,
                    color = "#28a745",
                    subtitle = s.title
                }).ToList();
                return Ok(events);
            }
            catch
            {
                return BadRequest("error");
            }
      
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(TblTimeSchedule schedule)
        {
            string res = "";

            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/TimeScheduler/Create";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(schedule), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return response.IsSuccessStatusCode ? Json(new { success = true, message = result })
                                         : Json(new { success = false, message = result });
                }

            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
    

        [HttpPost]
        public async Task<IActionResult> EditSchedule(TblTimeSchedule schedule)
        {
            HttpClient client = new HttpClient();
            var url = DBConn.HttpString + "/TimeScheduler/Edit";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
            StringContent content = new StringContent(JsonConvert.SerializeObject(schedule), Encoding.UTF8, "application/json");
            using (var response = await client.PostAsync(url, content))
            {
                string result = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode ? Json(new { success = true, message = result })
                                     : Json(new { success = false, message = result });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSchedule( int id)
        {
            if (id == 0) return Json(new { success = false, message = "Invalid ID" });

            string test = token_.GetValue();
            var url = $"{DBConn.HttpString}/TimeScheduler/Delete?id={id}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            HttpResponseMessage response = await client.PostAsync(url, null);
            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"API Response: {result}"); // Debugging response

            return response.IsSuccessStatusCode ? Json(new { success = true, message = result })
                                                : Json(new { success = false, message = result });
        }
        public class Params
        {
            public int? Usertype { get; set; }
            public DateTime? datefrom { get; set; }
            public DateTime? dateto { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> TotalRenderedHoursList(Params data)
        {
            string result = "";
            var list = new List<UserHoursReport>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/GetTotalRenderedHoursList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<UserHoursReport>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
    }
}
