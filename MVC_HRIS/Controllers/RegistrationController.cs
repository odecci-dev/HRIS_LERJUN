
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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVC_HRIS.Controllers
{
    public class RegistrationController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public RegistrationController(IOptions<AppSettings> appSettings,  QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }
        
        public IActionResult Index(FilterEmpId data)
        {
            return PartialView("Index");
        }
        public class EmployeeRegistrationModel
        {
            public string? EmployeeId { get; set; }
            public string? Password { get; set; }
            public string? Email { get; set; }

        }
        public class FilterEmpId
        {
            public string? EmployeeId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeRegistration(EmployeeRegistrationModel data)
        {
            
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/EmployeeRegistration";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    if (numericStatusCode == 200)
                    {
                        status = numericStatusCode.ToString();
                    }
                    else
                    {
                        status = await response.Content.ReadAsStringAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                string res=  ex.GetBaseException().ToString();
            }
            return Json(new { stats = status });
        }
    }
}
