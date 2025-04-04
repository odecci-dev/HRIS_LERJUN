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

namespace MVC_HRIS.Controllers
{
    public class FilingController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public FilingController(IOptions<AppSettings> appSettings, QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }
        public partial class TblOvertimeImportModel
        {

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
            public string? ConvertToOffset { get; set; }
            public string? DateCreated { get; set; }
            public int? LeaveId { get; set; }
            public int? Status { get; set; }
            public string? CreatedBy { get; set; }
            public string? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public string? DateDeleted { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            System.Text.Encoding.RegisterProvider(
            System.Text.CodePagesEncodingProvider.Instance);
            try { 
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

                        var data = new List<TblOvertimeImportModel>();

                        while (reader.Read())
                        {
                            i++;

                            if (i > 1) // Skipping header row
                            {
                                // Check if at least one column has data
                                if (string.IsNullOrWhiteSpace(reader.GetValue(0)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(1)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(2)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(3)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(4)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(5)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(6)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(7)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(8)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(9)?.ToString()) &&
                                    string.IsNullOrWhiteSpace(reader.GetValue(10)?.ToString())

                                    )
                                {
                                    continue; // Skip this row
                                }

                                // Process the row
                                data.Add(new TblOvertimeImportModel
                                {
                                    
                                    HoursFiled = reader.GetValue(5)?.ToString() ?? "",
                                    Remarks = reader.GetValue(6)?.ToString() ?? "",
                                    ConvertToLeave = reader.GetValue(7)?.ToString() ?? "",
                                    ConvertToOffset = reader.GetValue(8)?.ToString() ?? "",
                                    EmployeeNo = reader.GetValue(9)?.ToString() ?? "",
                                    CreatedBy = reader.GetValue(10)?.ToString() ?? "",
                                    Date = reader.GetValue(11)?.ToString() ?? "",
                                    StartDate = reader.GetValue(12)?.ToString() ?? "",
                                    EndDate = reader.GetValue(13)?.ToString() ?? "",
                                    StartTime = reader.GetValue(14)?.ToString() ?? "",
                                    EndTime = reader.GetValue(15)?.ToString() ?? "",
                                });
                            }
                        }
                        reader.Close();
                        System.IO.File.Delete(filename);

                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/Overtime/Import";

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
        public IActionResult Index()
        {
            string token = HttpContext.Session.GetString("Bearer");
            if (token == null)
            {
                return RedirectToAction("Index", "LogIn");
            }

            return View();
        }
       


    }
}
