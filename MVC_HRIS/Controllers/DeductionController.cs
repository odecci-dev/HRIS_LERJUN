
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
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using API_HRIS.Models;
using static MVC_HRIS.Controllers.TimeLogsController;

namespace MVC_HRIS.Controllers
{
    public class DeductionController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public DeductionController(IOptions<AppSettings> appSettings,  QueryValueService _token,
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

            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetSSSList()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Deduction/GetSSSList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblSSSModel> models = JsonConvert.DeserializeObject<List<TblSSSModel>>(response);

            return Json(new { draw = 1, data = models, recordFiltered = models?.Count, recordsTotal = models?.Count });
        }
        [HttpGet]
        public async Task<JsonResult> GetPagibigList()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Deduction/GetPagibigList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblPagIbigModel> models = JsonConvert.DeserializeObject<List<TblPagIbigModel>>(response);

            return Json(new { draw = 1, data = models, recordFiltered = models?.Count, recordsTotal = models?.Count });
        }
        [HttpGet]
        public async Task<JsonResult> GetPhilHealthList()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Deduction/GetPhilHealthList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblPhilHealthModel> models = JsonConvert.DeserializeObject<List<TblPhilHealthModel>>(response);

            return Json(new { draw = 1, data = models, recordFiltered = models?.Count, recordsTotal = models?.Count });
        }
        public class TaxModel
        {
            public decimal? Tax_From { get; set; }
            public decimal? Tax_To { get; set; }
            public decimal? PrescribeWithHoldingTax { get; set; }
            public string? TaxTypeName { get; set; }
        }
        [HttpGet]
        public async Task<JsonResult> GetTaxList()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Deduction/GetTaxList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TaxModel> models = JsonConvert.DeserializeObject<List<TaxModel>>(response);

            return Json(new { draw = 1, data = models, recordFiltered = models?.Count, recordsTotal = models?.Count });
        }

        public IActionResult DownloadPhilhealthHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Salary From";
                ws.Cells["B1"].Value = "Salary To";
                ws.Cells["C1"].Value = "Monthly Premium";
                ws.Cells["D1"].Value = "Employer Share";
                ws.Cells["E1"].Value = "Employee Share";

                ws.Cells["G1"].Value = "Employee No.";
                ws.Cells["H1"].Value = "UserID";

                ws.Cells["G2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["H2"].Value = HttpContext.Session.GetString("Id");
                // Enforce Boolean (Dropdown with TRUE/FALSE)
                for (var col = 1; col <= 12; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }

                ws.Cells["F1:F2"].Style.Font.Italic = true;
                ws.Cells["F1:F2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["F1"].Value = "All Fields are required";
                ws.Cells["F2"].Value = "Please follow the format shown in the first row.";
                
                ws.Cells.AutoFitColumns();

                int[] hideColumns = { 7, 8 };

                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true; // Hide column
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                ws.Cells["A:E"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock
                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Philhealth-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public IActionResult DownloadTaxHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Tax From";
                ws.Cells["B1"].Value = "Tax To";
                ws.Cells["C1"].Value = "Prescribe With Holding Tax";
                ws.Cells["D1"].Value = "Tax Type ID";
                ws.Cells["E1"].Value = "Tax Type";

                ws.Cells["G1"].Value = "Employee No.";
                ws.Cells["H1"].Value = "UserID";

                ws.Cells["G2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["H2"].Value = HttpContext.Session.GetString("Id");
                // Enforce Boolean (Dropdown with TRUE/FALSE)
                for (var col = 1; col <= 12; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }

                ws.Cells["F1:F2"].Style.Font.Italic = true;
                ws.Cells["F1:F2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["F1"].Value = "All Fields are required";
                ws.Cells["F2"].Value = "Please follow the format shown in the first row.";

                string sqlTask = $@"SELECT TOP (1000) [Id],[Name] FROM [ODC_HRIS].[dbo].[TblTaxTypeModel]";
                var dataSet = db.SelectDb(sqlTask);

                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }

                DataTable dt = dataSet.Tables[0];
                int ctr = 2;
                var validation = ws.DataValidations.AddListValidation("E2:E1000");
                foreach (DataRow dr in dt.Rows)
                {
                    ws.Cells["R" + ctr].Value = dr["Name"].ToString();
                    ws.Cells["S" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    validation.Formula.Values.Add(dr["Name"].ToString());
                    ctr++;
                }
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = "Invalid Selection";
                validation.Error = "Please select a valid option from the dropdown list.";
                int ctrTierId = 50;
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["D" + i].Formula = "=IFERROR(VLOOKUP(E" + i + ",R2:S10,2,FALSE),\"\")";
                }


                ws.Cells.AutoFitColumns();

                int[] hideColumns = { 4, 7, 8, 18, 19 };

                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true; // Hide column
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                ws.Cells["A:E"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock
                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Tax-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public IActionResult DownloaSSSHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Compensation From";
                ws.Cells["B1"].Value = "Compensation To";
                ws.Cells["C1"].Value = "Employer RegularSS";
                ws.Cells["D1"].Value = "Employer MPF";
                ws.Cells["E1"].Value = "Employer EC";
                ws.Cells["F1"].Value = "Employer Total";
                ws.Cells["G1"].Value = "Employee RegularSS";
                ws.Cells["H1"].Value = "Employee MPF";
                ws.Cells["I1"].Value = "Employee Total";
                ws.Cells["J1"].Value = "Total Contribution";

                ws.Cells["M1"].Value = "Employee No.";
                ws.Cells["N1"].Value = "UserID";

                ws.Cells["M2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["N2"].Value = HttpContext.Session.GetString("Id");
                // Enforce Boolean (Dropdown with TRUE/FALSE)
                for (var col = 1; col <= 12; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }

                ws.Cells["L1:L2"].Style.Font.Italic = true;
                ws.Cells["L1:L2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["L1"].Value = "All Fields are required";
                ws.Cells["L2"].Value = "Please follow the format shown in the first row.";

                ws.Cells.AutoFitColumns();

                int[] hideColumns = { 13, 14 };

                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true; // Hide column
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                ws.Cells["A:J"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock
                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "SSS-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public IActionResult DownloaPagibigHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Tax From";
                ws.Cells["B1"].Value = "Tax To";
                ws.Cells["C1"].Value = "Employee Contribution Rate";
                ws.Cells["D1"].Value = "Employer Contribution Rate";
                ws.Cells["E1"].Value = "Total Contribution";

                ws.Cells["M1"].Value = "Employee No.";
                ws.Cells["N1"].Value = "UserID";

                ws.Cells["M2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["N2"].Value = HttpContext.Session.GetString("Id");
                // Enforce Boolean (Dropdown with TRUE/FALSE)
                for (var col = 1; col <= 12; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }

                ws.Cells["L1:L2"].Style.Font.Italic = true;
                ws.Cells["L1:L2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["L1"].Value = "All Fields are required";
                ws.Cells["L2"].Value = "Please follow the format shown in the first row.";

                ws.Cells.AutoFitColumns();

                int[] hideColumns = {6, 7, 8, 9, 10, 13, 14 };

                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true; // Hide column
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                ws.Cells["A:E"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock
                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Pagibig-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public partial class TblPhilhealthImportModel
        {
            public string? Salary_From { get; set; }
            public string? Salary_To { get; set; }
            public string? Monthly_Premium { get; set; }
            public string? Employer_Share { get; set; }
            public string? Employee_Share { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> PhilheathIndex(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
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

                        var data = new List<TblPhilhealthImportModel>();

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
                                data.Add(new TblPhilhealthImportModel
                                {

                                    Salary_From = reader.GetValue(0)?.ToString() ?? "",
                                    Salary_To = reader.GetValue(1)?.ToString() ?? "",
                                    Monthly_Premium = reader.GetValue(2)?.ToString() ?? "",
                                    Employer_Share = reader.GetValue(3)?.ToString() ?? "",
                                    Employee_Share = reader.GetValue(4)?.ToString() ?? "",
                                });
                            }
                        }
                        reader.Close();
                        System.IO.File.Delete(filename);

                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/Deduction/ImportPhilhealth";

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
        public partial class TblTaxImportModel
        {
            public string? Tax_From { get; set; }
            public string? Tax_To { get; set; }
            public string? PrescribeWithHoldingTax { get; set; }
            public string? TaxTypeId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> TaxIndex(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
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

                        var data = new List<TblTaxImportModel>();

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
                                data.Add(new TblTaxImportModel
                                {

                                    Tax_From = reader.GetValue(0)?.ToString() ?? "",
                                    Tax_To = reader.GetValue(1)?.ToString() ?? "",
                                    PrescribeWithHoldingTax = reader.GetValue(2)?.ToString() ?? "",
                                    TaxTypeId = reader.GetValue(3)?.ToString() ?? "",
                                });
                            }
                        }
                        reader.Close();
                        System.IO.File.Delete(filename);

                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/Deduction/ImportTax";

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

        public partial class TblSSSImportModel
        {
            public string? Compensation_From { get; set; }
            public string? Compensation_To { get; set; }
            public string? Employer_RegularSS { get; set; }
            public string? Employer_MPF { get; set; }
            public string? Employer_EC { get; set; }
            public string? Employer_Total { get; set; }
            public string? Employee_RegularSS { get; set; }
            public string? Employee_MPF { get; set; }
            public string? Employee_Total { get; set; }
            public string? Total_Contribution { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> SSSIndex(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
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

                        var data = new List<TblSSSImportModel>();

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
                                data.Add(new TblSSSImportModel
                                {

                                    Compensation_From = reader.GetValue(0)?.ToString() ?? "",
                                    Compensation_To = reader.GetValue(1)?.ToString() ?? "",
                                    Employer_RegularSS = reader.GetValue(2)?.ToString() ?? "",
                                    Employer_MPF = reader.GetValue(3)?.ToString() ?? "",
                                    Employer_EC = reader.GetValue(4)?.ToString() ?? "",
                                    Employer_Total = reader.GetValue(5)?.ToString() ?? "",
                                    Employee_RegularSS = reader.GetValue(6)?.ToString() ?? "",
                                    Employee_MPF = reader.GetValue(7)?.ToString() ?? "",
                                    Employee_Total = reader.GetValue(8)?.ToString() ?? "",
                                    Total_Contribution = reader.GetValue(9)?.ToString() ?? "",
                                });
                            }
                        }
                        reader.Close();
                        System.IO.File.Delete(filename);

                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/Deduction/ImportSSS";

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
        public partial class TblPagibigImportModel
        {
            public string? Tax_From { get; set; }
            public string? Tax_To { get; set; }
            public string? Employee_Contribution_Rate { get; set; }
            public string? Employer_Contribution_Rate { get; set; }
            public string? Total_Contribution { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> PagibigIndex(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
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

                        var data = new List<TblPagibigImportModel>();

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
                                data.Add(new TblPagibigImportModel
                                {

                                    Tax_From = reader.GetValue(0)?.ToString() ?? "",
                                    Tax_To = reader.GetValue(1)?.ToString() ?? "",
                                    Employee_Contribution_Rate = reader.GetValue(2)?.ToString() ?? "",
                                    Employer_Contribution_Rate = reader.GetValue(3)?.ToString() ?? "",
                                    Total_Contribution = reader.GetValue(4)?.ToString() ?? "",
                                });
                            }
                        }
                        reader.Close();
                        System.IO.File.Delete(filename);

                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/Deduction/ImportPagibig";

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
    }
}
