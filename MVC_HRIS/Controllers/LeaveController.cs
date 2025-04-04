
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
    public class LeaveController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public LeaveController(IOptions<AppSettings> appSettings, QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }



        public IActionResult LeaveTypeMaintenance()
        {
            return View("_LeaveTypeMaintenance");
        }

        public IActionResult LeaveFiling()
        {
            return PartialView("LeaveFiling");
        }
        public IActionResult Leave()
        {
            return PartialView("_Leave");
        }
        public IActionResult LeaveApproval()
        {
            return PartialView("LeaveApproval");
        }
        [HttpGet]
        public async Task<JsonResult> GetLeaveTypeListtOption()
        {
            //string result = "";

            var list = new List<TblLeaveTypeModel>();
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Leave/LeaveTypeList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblLeaveTypeModel> models = JsonConvert.DeserializeObject<List<TblLeaveTypeModel>>(response);
            return new(models);
        }
        public class LeaveRequestListParam
        {
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? UserId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetLeaveRequestList(LeaveRequestListParam data)
        {
            string result = "";
            var list = new List<TblLeaveRequestModel>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Leave/LeaveRequestList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblLeaveRequestModel>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingLeaveRequestList(LeaveRequestListParam data)
        {
            string result = "";
            var list = new List<TblLeaveRequestModel>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Leave/PendingLeaveRequestList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblLeaveRequestModel>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
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
            public string? Date { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? DaysFiled { get; set; }
            public string? LeaveTypeId { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetCheckedLeaveRequestList(CheckedLeaveRequestListParam data)
        {
            string result = "";
            var list = new List<TblLeaveRequestVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Leave/CheckedLeaveRequestList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblLeaveRequestVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }

        [HttpPost]
        public async Task<IActionResult> SaveLR(TblLeaveRequestModel data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Leave/save";

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
        public async Task<IActionResult> MultiApproval(multiApprovalParam data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Leave/MultiOTUpdateStatus";

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
