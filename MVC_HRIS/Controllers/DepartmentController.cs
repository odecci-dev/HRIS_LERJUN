
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
using API_HRIS.ApplicationModel;
using static MVC_HRIS.Controllers.PositionController;
using API_HRIS.Manager;
using API_HRIS.Models;
using System.Net;

namespace MVC_HRIS.Controllers
{
    public class DepartmentController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public DepartmentController(IOptions<AppSettings> appSettings, QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            //string  token = HttpContext.Session.GetString("Bearer");
            //if (token == null)
            //{
            //    return RedirectToAction("Index", "LogIn");
            //}
            var json = HttpContext.Session.GetString("MyList");
            var models = string.IsNullOrEmpty(json)
                ? new List<TblModulesModel>()
                : JsonConvert.DeserializeObject<List<TblModulesModel>>(json);

            // Pass the data to the view
            ViewBag.SideBarModules = models;
            return View();
        }
        public class FilterDepartment
        {

            public string? Department { get; set; }
            public int page { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetDepartmentList(FilterDepartment data)
        {
            string result = "";
            var list = new List<DepartmentPaginationModel>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Department/DepartmentPaginationList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<DepartmentPaginationModel>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetDepartmentList()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Department/DepartmentList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblDeparmentModel> models = JsonConvert.DeserializeObject<List<TblDeparmentModel>>(response);
            return Json(new { draw = 1, data = models, recordFiltered = models?.Count, recordsTotal = models?.Count });
        }

        [HttpPost]
        public async Task<IActionResult> SaveDepartment(TblDeparmentModel data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Department/save";

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
