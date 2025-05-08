
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
using API_HRIS.Models;
using System.Net;
using API_HRIS.Manager;
using System.Drawing.Imaging;
using static AOPC.Controllers.LogInController;
using API_HRIS.ApplicationModel;
using NPOI.HPSF;
using static MVC_HRIS.Controllers.DeductionController;
using DocumentFormat.OpenXml.Presentation;
using System.Globalization;

namespace MVC_HRIS.Controllers
{
    public class EmployeeController : Controller
    {
        string status = "";
        string res = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        private IWebHostEnvironment Environment;
        public EmployeeController(IOptions<AppSettings> appSettings,  QueryValueService _token, IWebHostEnvironment _environment,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            Environment = _environment;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
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
        public IActionResult AddEmployee()
        {
            string token = HttpContext.Session.GetString("Bearer");
            if (token == "" || token == null)
            {
                return RedirectToAction("Index", "LogIn");
            }
            return View();
        }
        public class Filter
        {
            public string? Username { get; set; }
            public string? Department { get; set; }
            public int page { get; set; }
        }
        [HttpPost]
        public IActionResult SubmitValue(string value)
        {
            // Process the value
            // For example, redirect to another action with the value as a parameter
            return RedirectToAction("DisplayValue", new { id = value });
        }

        public IActionResult DisplayValue(string id)
        {
            // Use the value (id) as needed
            return View((object)id);
        }
        [HttpPost]
        public async Task<IActionResult> GetEmployeeDetails(IdFilter data)
        {
            string result = "";
            var list = new List<GetAllUserDetailsResult>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/EmployeeFilteredById";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<GetAllUserDetailsResult>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> GetECEmployeeDetails(IdFilter data)
        {
            string result = "";
            var list = new List<TblEmergencyContactsModel>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/ECEmployeeFilteredById";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblEmergencyContactsModel>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        [HttpGet]
        public async Task<JsonResult> GetStatusType()
        {

            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Employee/StatusTypeList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblStatusModel> models = JsonConvert.DeserializeObject<List<TblStatusModel>>(response);
            return new(models);
        }
        public class EmployeeListViewModel
        {

            public int? Id { get; set; }
            public string? Department { get; set; }
            public string? UserType { get; set; }
            public string? EmployeeType { get; set; }
            public string? EmployeeId { get; set; }
            public string? Lname { get; set; }
            public string? Fname { get; set; }
            public string? Mname { get; set; }
            public string? Fullname { get; set; }
            public string? Suffix { get; set; }
            public string? Email { get; set; }
            public string? Cno { get; set; }
            public string? Gender { get; set; }
            public string? DateStarted { get; set; }
            public string? CreatedBy { get; set; }
            public string? Address { get; set; }
            public string? SalaryType { get; set; }
            public string? PayrollType { get; set; }
            public string? Status { get; set; }
            public string? Position { get; set; }
            public string? FilePath { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public int? PositionLevelId { get; set; }
            public string? PositionLevel { get; set; }
            public int? ManagerId { get; set; }
            public string? Rate { get; set; }
            public string? DaysInMonth { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetDataRegistrationList( Filter data)
        {
            string result = "";
            var list = new List<EmployeeListViewModel>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/EmployeePaginationList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<EmployeeListViewModel>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });

        }
        public class EmployeeViewModel
        {

            public string? Id { get; set; }
            public string Department { get; set; }
            public string UserType { get; set; }
            public string EmployeeType { get; set; }
            public string Position { get; set; }
            public string Lname { get; set; }
            public string Fname { get; set; }
            public string Mname { get; set; }
            public string? Suffix { get; set; }
            public string Email { get; set; }
            public string Cno { get; set; }
            public string Gender { get; set; }
            public string DateStarted { get; set; }
            public string CreatedBy { get; set; }
            public string Address { get; set; }
            public string SalaryType { get; set; }
            public string PayrollType { get; set; }
            public string Status { get; set; }
            public string? FilePath { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public int? PositionLevelId { get; set; }
            public int? ManagerId { get; set; }
            public string Rate { get; set; }
            public string DaysInMonth { get; set; }
            public string? SSS_Number { get; set; }
            public string? PagIbig_MID_Number { get; set; }
            public string? PhilHealth_Number { get; set; }
            public string? Tax_Identification_Number { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> SaveEmployee(EmployeeViewModel data)
        {
            try
            {
            
                HttpClient client = new HttpClient();
                //var url = "http://localhost:64181/Employee/saveemployee";
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                //StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var url = DBConn.HttpString + "/Employee/saveemployee";
                var token = _global.GenerateToken(data.Fname, _appSettings.Key.ToString());
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
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { status = res });
        }
        [HttpPost]
        public async Task<IActionResult> UploadDocuments()
        {
            var files = Request.Form.Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var filePath = Path.Combine("wwwroot/employeedocuments", file.FileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }

                return Ok("Files uploaded successfully.");
            }

            return BadRequest("No files selected.");
        }
        [HttpPost]
        public async Task<IActionResult> UploadOtherDocuments()
        {
            var files = Request.Form.Files;

            if (files == null || files.Count == 0)
            {
                return BadRequest("No files selected.");
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "employeedocuments");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var sanitizedFileName = Path.GetFileName(file.FileName); // prevent directory traversal
                    var filePath = Path.Combine(uploadPath, sanitizedFileName);

                    // Optional: Check allowed extensions
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".docx" }; // example
                    var ext = Path.GetExtension(sanitizedFileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(ext))
                    {
                        return BadRequest($"File extension {ext} is not allowed.");
                    }

                    // Optional: Avoid overwriting by renaming (if needed)
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return Ok("Files uploaded successfully.");
        }

        public async Task<IActionResult> UploadFile(List<IFormFile> postedFiles)
        {
            int i;
            var stream = (dynamic)null;
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            int ctr = 0;
            string img = "";
            try
            {
                for (i = 0; i < Request.Form.Files.Count; i++)
            {
                if (Request.Form.Files[i].Length > 0)
                {
                    try
                    {
                        string wwwRootPath = Environment.WebRootPath;
                        //var uploadsFolder = DBConn.Path;
                        var uploadsFolder = Path.Combine(wwwRootPath, "img");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        List<string> uploadedFiles = new List<string>();


                        var image = System.Drawing.Image.FromStream(Request.Form.Files[i].OpenReadStream());
                        var resized = new Bitmap(image, new System.Drawing.Size(400, 400));

                        using var imageStream = new MemoryStream();
                        resized.Save(imageStream, ImageFormat.Jpeg);
                        var imageBytes = imageStream;
                        string sql = "";

                        //var id = table.Rows[0]["OfferingID"].ToString();
                        string getextension = Path.GetExtension(Request.Form.Files[i].FileName);
                        //string MyUserDetailsIWantToAdd = str + ".jpg";


                        img += "https://www.alfardanoysterprivilegeclub.com/assets/img/" + Request.Form.Files[i].FileName + ";";

                        string file = Path.Combine(uploadsFolder, Request.Form.Files[i].FileName.Trim());
                        FileInfo f1 = new FileInfo(file);

                        stream = new FileStream(file, FileMode.Create);
                        await Request.Form.Files[i].CopyToAsync(stream);
                    }
                    catch (Exception ex)
                    {
                        status = "Error! " + ex.GetBaseException().ToString();
                    }

                }
                ctr++;
                stream.Close();
                stream.Dispose();
            }
            }
            catch (Exception ex)
            {
                status = "Error! " + ex.GetBaseException().ToString();
            }
            if (Request.Form.Files.Count == 0) { status = "Error"; }
            return Json(new { stats = status });
        }
        [HttpPost]
        public async Task<IActionResult> deleteemployee(DeletionModel data)
        {
            try
            {
               
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/delete";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
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
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { stats = status });
        }

        public class UnregisteredUserEmailRequest
        {
            public string[] Name { get; set; }
            public string[] Email { get; set; }
            public string[] Username { get; set; }
            public string[] Password { get; set; }
        }
        //[HttpPost]
        //public async Task<IActionResult> EmailUnregisterUser(UnregisteredUserEmailRequest data)
        //{
        //    var list = new List<UnregisteredUserEmailRequest>();
        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        var url = DBConn.HttpString + "/Employee/EmailUnregisterUser";
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        //        using (var response = await client.PostAsync(url, content))
        //        {
        //            string res = await response.Content.ReadAsStringAsync();
        //            list = JsonConvert.DeserializeObject<List<UnregisteredUserEmailRequest>>(res);

        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        string status = ex.GetBaseException().ToString();
        //    }
        //    return Json(list);
        //}
        [HttpPost]
        public async Task<IActionResult> EmailUnregisterUser(UnregisteredUserEmailRequest data)
        {
            string result = "";
            var list = new List<UnregisteredUserEmailRequest>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/EmailUnregisterUser";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<UnregisteredUserEmailRequest>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
            //return View();
        }

        public class Manager
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetManagerSelect()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Employee/GetManager";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<Manager> models = JsonConvert.DeserializeObject<List<Manager>>(response);
            return Json(models);
        }
        [HttpPost]
        public async Task<IActionResult> SaveEmergencyContact(TblEmergencyContactsModel data)
        {
            string result = "";
            var list = new List<TblEmergencyContactsModel>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/EmergencyContact";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblEmergencyContactsModel>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
            //return View();
        }
        
        
        [HttpPost]
        public async Task<IActionResult> RequiredDocuments(List<tbl_UsersRequiredDocuments> data)
        {
            
            if (data[0].FileType == "" || data[0].FileType == null)
            {
                if (string.IsNullOrWhiteSpace(data[0].FileName))
                    return BadRequest("Invalid file name.");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "employeedocuments", data[0].FileName);


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    //return Ok("File deleted successfully.");
                }
            }
            string result = "";
            var list = new List<tbl_UsersRequiredDocuments>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/SaveRequiredDocuments";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<tbl_UsersRequiredDocuments>>(res);
                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        public class RequiredDocumentsParam
        {
            public int UserId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> PostRequiredDocuments(RequiredDocumentsParam data)
        {
            string result = "";
            var list = new List<tbl_UsersRequiredDocuments>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Employee/PostRequiredDocuments";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<tbl_UsersRequiredDocuments>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        public IActionResult DownloadEmployeeHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "First Name";
                ws.Cells["B1"].Value = "Last Name";
                ws.Cells["C1"].Value = "Middle Name";
                ws.Cells["D1"].Value = "Suffix";
                ws.Cells["E1"].Value = "Gender";
                ws.Cells["F1"].Value = "Email";
                ws.Cells["G1"].Value = "Department";
                ws.Cells["H1"].Value = "Department Id";
                ws.Cells["I1"].Value = "Position";
                ws.Cells["J1"].Value = "Position Id";
                ws.Cells["K1"].Value = "Position Level";
                ws.Cells["L1"].Value = "Position Level Id";
                ws.Cells["M1"].Value = "User Type";
                ws.Cells["N1"].Value = "User Type Id";
                ws.Cells["O1"].Value = "Employee Type";
                ws.Cells["P1"].Value = "Employee Type Id";
                ws.Cells["Q1"].Value = "Manager";
                ws.Cells["R1"].Value = "Manager Id";
                ws.Cells["S1"].Value = "Date Hired";
                ws.Cells["T1"].Value = "SSS Number";
                ws.Cells["U1"].Value = "PagIbig MID Number";
                ws.Cells["V1"].Value = "PhilHealth MID Number";
                ws.Cells["W1"].Value = "Tax Identification Number";
                ws.Cells["X1"].Value = "Salary Type";
                ws.Cells["Y1"].Value = "Salary Type Id";
                ws.Cells["Z1"].Value = "Payroll Type";
                ws.Cells["AA1"].Value = "Payroll Type Id";
                ws.Cells["AB1"].Value = "Rate";
                ws.Cells["AC1"].Value = "Days In Month";
                ws.Cells["AD1"].Value = "Country of Birth";
                ws.Cells["AE1"].Value = "Region";
                ws.Cells["AF1"].Value = "Province";
                ws.Cells["AG1"].Value = "Municipality";
                ws.Cells["AH1"].Value = "Barangay";
                ws.Cells["AI1"].Value = "Emergency Contact Name";
                ws.Cells["AJ1"].Value = "Relationship";
                ws.Cells["AK1"].Value = "Emergency Contact Number";
                ws.Cells["AL1"].Value = "Employee Contact Number";


                ws.Cells["AN1"].Value = "Employee No.";
                ws.Cells["AO1"].Value = "UserID";

                ws.Cells["AN2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["AO2"].Value = HttpContext.Session.GetString("Id");

                for (var col = 1; col <= 39; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }

                ws.Cells["AM1:AM2"].Style.Font.Italic = true;
                ws.Cells["AM1:AM2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["AM1"].Value = "All Fields are required";
                ws.Cells["AM2"].Value = "Please follow the format shown in the first row.";

                //Department Start
                ws.Cells["AP1"].Value = "DepartmentName";
                ws.Cells["AQ1"].Value = "DepartmentID";
                string sqlDepartment = $@"SELECT [Id],[DepartmentName] FROM [ODC_HRIS].[dbo].[tbl_DeparmentModel] WHERE DeleteFlag = 0";
                var dataSet = db.SelectDb(sqlDepartment);
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable dt = dataSet.Tables[0];
                int ctr = 2;
                var validation = ws.DataValidations.AddListValidation("G2:G1000");
                foreach (DataRow dr in dt.Rows)
                {
                    ws.Cells["AP" + ctr].Value = dr["DepartmentName"].ToString();
                    ws.Cells["AQ" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    validation.Formula.Values.Add(dr["DepartmentName"].ToString());
                    ctr++;
                }
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = "Invalid Selection";
                validation.Error = "Please select a valid option from the dropdown list.";
                int ctrTierId = 1000;
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["H" + i].Formula = "=IFERROR(VLOOKUP(G" + i + ",AP2:AQ"+ ctr+",2,FALSE),\"\")";

                }
                //Department End
                //Position Start
                ws.Cells["AR1"].Value = "Position";
                ws.Cells["AS1"].Value = "PositionID";
                string sqlPosition = $@"SELECT [Id],[Name] FROM [ODC_HRIS].[dbo].[tbl_PositionModel] WHERE DeleteFlag = 0";
                var positiondataSet = db.SelectDb(sqlPosition);
                if (positiondataSet == null || positiondataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable positiondt = positiondataSet.Tables[0];
                var positionvalidation = ws.DataValidations.AddListValidation("I2:I1000");

                ctr = 2;
                foreach (DataRow dr in positiondt.Rows)
                {
                    ws.Cells["AR" + ctr].Value = dr["Name"].ToString();
                    ws.Cells["AS" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    positionvalidation.Formula.Values.Add(dr["Name"].ToString());
                    ctr++;
                }
                positionvalidation.ShowErrorMessage = true;
                positionvalidation.ErrorTitle = "Invalid Selection";
                positionvalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["J" + i].Formula = "=IFERROR(VLOOKUP(I" + i + ",AR2:AS"+ ctr+",2,FALSE),\"\")";

                }
                //Position End
                //Position Level Start
                ws.Cells["AT1"].Value = "PositionLevel";
                ws.Cells["AU1"].Value = "PositionLevelID";
                string sqlPositionLvl = $@"SELECT [Id],[Level] FROM [ODC_HRIS].[dbo].[tbl_PositionLevel] WHERE DeleteFlag = 0";
                var positionLvldataSet = db.SelectDb(sqlPositionLvl);
                if (positionLvldataSet == null || positionLvldataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable positionLvldt = positionLvldataSet.Tables[0];
                var positionLvlvalidation = ws.DataValidations.AddListValidation("K2:K1000");

                ctr = 2;
                foreach (DataRow dr in positionLvldt.Rows)
                {
                    ws.Cells["AT" + ctr].Value = dr["Level"].ToString();
                    ws.Cells["AU" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    positionLvlvalidation.Formula.Values.Add(dr["Level"].ToString());
                    ctr++;
                }
                positionLvlvalidation.ShowErrorMessage = true;
                positionLvlvalidation.ErrorTitle = "Invalid Selection";
                positionLvlvalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["L" + i].Formula = "=IFERROR(VLOOKUP(K" + i + ",AT2:AU" + ctr + ",2,FALSE),\"\")";

                }
                //Position Level End

                //User Type Start
                ws.Cells["AV1"].Value = "UserType";
                ws.Cells["AW1"].Value = "UserTypeID";
                string sql = $@"SELECT [Id],[UserType] FROM [ODC_HRIS].[dbo].[tbl_UserType] WHERE Status = 1";
                dataSet = db.SelectDb(sql);
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable userTypedt = dataSet.Tables[0];
                var userTypevalidation = ws.DataValidations.AddListValidation("M2:M1000");

                ctr = 2;
                foreach (DataRow dr in userTypedt.Rows)
                {
                    ws.Cells["AV" + ctr].Value = dr["UserType"].ToString();
                    ws.Cells["AW" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    userTypevalidation.Formula.Values.Add(dr["UserType"].ToString());
                    ctr++;
                }
                userTypevalidation.ShowErrorMessage = true;
                userTypevalidation.ErrorTitle = "Invalid Selection";
                userTypevalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["N" + i].Formula = "=IFERROR(VLOOKUP(M" + i + ",AV2:AW" + ctr + ",2,FALSE),\"\")";

                }
                //User Type End
                //Employee Type Start
                ws.Cells["AX1"].Value = "EmployeeType";
                ws.Cells["AY1"].Value = "EmployeeTypeID";
                sql = $@"SELECT [Id],[Title] FROM [ODC_HRIS].[dbo].[tbl_EmployeeType] WHERE DeleteFlag = 0 ";
                dataSet = db.SelectDb(sql);
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable employeeTypedt = dataSet.Tables[0];
                var employeeTypevalidation = ws.DataValidations.AddListValidation("O2:O1000");

                ctr = 2;
                foreach (DataRow dr in employeeTypedt.Rows)
                {
                    ws.Cells["AX" + ctr].Value = dr["Title"].ToString();
                    ws.Cells["AY" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    employeeTypevalidation.Formula.Values.Add(dr["Title"].ToString());
                    ctr++;
                }
                employeeTypevalidation.ShowErrorMessage = true;
                employeeTypevalidation.ErrorTitle = "Invalid Selection";
                employeeTypevalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["P" + i].Formula = "=IFERROR(VLOOKUP(O" + i + ",AX2:AY" + ctr + ",2,FALSE),\"\")";

                }
                //Employee Type End


                //Manager Start
                ws.Cells["AZ1"].Value = "Manager";
                ws.Cells["BA1"].Value = "ManagerID";
                sql = $@"SELECT [Id], [Fullname] FROM [ODC_HRIS].[dbo].[tbl_UsersModel] WHERE PositionLevelId IN (5,6) AND Delete_Flag = 0";
                dataSet = db.SelectDb(sql);
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable managerdt = dataSet.Tables[0];
                var managervalidation = ws.DataValidations.AddListValidation("Q2:Q1000");

                ctr = 2;
                foreach (DataRow dr in managerdt.Rows)
                {
                    ws.Cells["AZ" + ctr].Value = dr["Fullname"].ToString();
                    ws.Cells["BA" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    managervalidation.Formula.Values.Add(dr["Fullname"].ToString());
                    ctr++;
                }
                managervalidation.ShowErrorMessage = true;
                managervalidation.ErrorTitle = "Invalid Selection";
                managervalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["R" + i].Formula = "=IFERROR(VLOOKUP(Q" + i + ",AZ2:BA" + ctr + ",2,FALSE),\"\")";

                }
                //Manager End

                //Salary Type Start
                ws.Cells["BB1"].Value = "SalaryType";
                ws.Cells["BC1"].Value = "SalaryTypeID";
                sql = $@"SELECT [Id],[SalaryType] FROM [ODC_HRIS].[dbo].[tbl_SalaryType] WHERE DeleteFlag = 0";
                dataSet = db.SelectDb(sql);
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable salaryTypedt = dataSet.Tables[0];
                var salaryTypevalidation = ws.DataValidations.AddListValidation("X2:X1000");

                ctr = 2;
                foreach (DataRow dr in salaryTypedt.Rows)
                {
                    ws.Cells["BB" + ctr].Value = dr["SalaryType"].ToString();
                    ws.Cells["BC" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    salaryTypevalidation.Formula.Values.Add(dr["SalaryType"].ToString());
                    ctr++;
                }
                salaryTypevalidation.ShowErrorMessage = true;
                salaryTypevalidation.ErrorTitle = "Invalid Selection";
                salaryTypevalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["Y" + i].Formula = "=IFERROR(VLOOKUP(X" + i + ",BB2:BC" + ctr + ",2,FALSE),\"\")";

                }
                //Salary Type End

                //Payroll Type Start
                ws.Cells["BD1"].Value = "PayrollType";
                ws.Cells["BE1"].Value = "PayrollTypeID";
                sql = $@"SELECT [Id],[PayrollType] FROM [ODC_HRIS].[dbo].[tbl_PayrollType] WHERE DeleteFlag = 0";
                dataSet = db.SelectDb(sql);
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No Task found in the database.");
                }
                DataTable payrollTypedt = dataSet.Tables[0];
                var payrollTypevalidation = ws.DataValidations.AddListValidation("Z2:Z1000");

                ctr = 2;
                foreach (DataRow dr in payrollTypedt.Rows)
                {
                    ws.Cells["BD" + ctr].Value = dr["PayrollType"].ToString();
                    ws.Cells["BE" + ctr].Value = dr["Id"].ToString();
                    // Add a dropdown list in A2 (below the header)
                    payrollTypevalidation.Formula.Values.Add(dr["PayrollType"].ToString());
                    ctr++;
                }
                payrollTypevalidation.ShowErrorMessage = true;
                payrollTypevalidation.ErrorTitle = "Invalid Selection";
                payrollTypevalidation.Error = "Please select a valid option from the dropdown list.";
                for (int i = 2; i < ctrTierId; i++)
                {
                    ws.Cells["AA" + i].Formula = "=IFERROR(VLOOKUP(Z" + i + ",BD2:BE" + ctr + ",2,FALSE),\"\")";

                }
                //Payroll Type End

                ws.Cells.AutoFitColumns();

                int[] hideColumns = { 8, 10, 12, 14, 16, 18, 25, 27, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 };

                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true; // Hide column
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                ws.Cells["A:AL"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock
                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Employee-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        public class ImportEmployeeViewModel
        {

            public string? Id { get; set; }
            public string? Department { get; set; }
            public string? UserType { get; set; }
            public string? EmployeeType { get; set; }
            public string? Position { get; set; }
            public string? Lname { get; set; }
            public string? Fname { get; set; }
            public string? Mname { get; set; }
            public string? Suffix { get; set; }
            public string? Email { get; set; }
            public string? Cno { get; set; }
            public string? Gender { get; set; }
            public string? DateStarted { get; set; }
            public string? CreatedBy { get; set; }
            public string? Address { get; set; }
            public string? SalaryType { get; set; }
            public string? PayrollType { get; set; }
            public string? Status { get; set; }
            public string? FilePath { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public int? PositionLevelId { get; set; }
            public int? ManagerId { get; set; }
            public string Rate { get; set; }
            public string DaysInMonth { get; set; }
            public string? SSS_Number { get; set; }
            public string? PagIbig_MID_Number { get; set; }
            public string? PhilHealth_Number { get; set; }
            public string? Tax_Identification_Number { get; set; }
            //EmergencyContacts
            public string? Name { get; set; }

            public string? Relationship { get; set; }

            public string? PhoneNumber { get; set; }

        }
        static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpPost]
        public async Task<IActionResult> ImportEmployee(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
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
                        var data = new List<ImportEmployeeViewModel>();
                        
                        //var EmergencyContactData = new List<EmployeeViewModel>();
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

                                var address =
                                    (reader.GetValue(29)?.ToString().Trim() ?? "") + ", " +
                                    (reader.GetValue(30)?.ToString().Trim() ?? "") + ", " +
                                    (reader.GetValue(31)?.ToString().Trim() ?? "") + ", " +
                                    (reader.GetValue(32)?.ToString().Trim() ?? "") + ", " +
                                    (reader.GetValue(33)?.ToString().Trim() ?? "");
                                string pass = GetRandomString(8);
                                var username =
                                (reader.GetValue(0)?.ToString().Trim().ToLower(CultureInfo.InvariantCulture) ?? "") + "." +
                                (reader.GetValue(1)?.ToString().Trim().ToLower(CultureInfo.InvariantCulture) ?? "");// Process the row
                                data.Add(new ImportEmployeeViewModel
                                {
                                    Fname = reader.GetValue(0)?.ToString().Trim() ?? "",
                                    Lname = reader.GetValue(1)?.ToString().Trim() ?? "",
                                    Mname = reader.GetValue(2)?.ToString().Trim() ?? "",
                                    Suffix = reader.GetValue(3)?.ToString().Trim() ?? "",
                                    Gender = reader.GetValue(4)?.ToString().Trim() ?? "",
                                    Username = username,
                                    Password = pass,
                                    Email = reader.GetValue(5)?.ToString().Trim() ?? "",
                                    Department = reader.GetValue(7)?.ToString().Trim() ?? "",
                                    Position = reader.GetValue(9)?.ToString().Trim() ?? "",
                                    PositionLevelId = reader.IsDBNull(11) ? null : Convert.ToInt32(reader.GetValue(11)),
                                    UserType = reader.GetValue(13)?.ToString().Trim() ?? "",
                                    EmployeeType = reader.GetValue(15)?.ToString().Trim() ?? "",
                                    ManagerId = reader.IsDBNull(17) ? null : Convert.ToInt32(reader.GetValue(17)),
                                    DateStarted = reader.GetValue(18)?.ToString().Trim() ?? "",
                                    SSS_Number = reader.GetValue(19)?.ToString().Trim() ?? "",
                                    PagIbig_MID_Number = reader.GetValue(20)?.ToString().Trim() ?? "",
                                    PhilHealth_Number = reader.GetValue(21)?.ToString().Trim() ?? "",
                                    Tax_Identification_Number = reader.GetValue(22)?.ToString().Trim() ?? "",
                                    SalaryType = reader.GetValue(24)?.ToString().Trim() ?? "",
                                    PayrollType = reader.GetValue(26)?.ToString().Trim() ?? "",
                                    Rate = reader.GetValue(27)?.ToString().Trim() ?? "",
                                    DaysInMonth = reader.GetValue(28)?.ToString() ?? "",
                                    Address = address,
                                    Name = reader.GetValue(34)?.ToString() ?? "",
                                    Relationship = reader.GetValue(35)?.ToString() ?? "",
                                    PhoneNumber = reader.GetValue(36)?.ToString() ?? "",
                                    Cno = reader.GetValue(37)?.ToString() ?? "",
                                    Status = "1003"
                                });
                            }

                        }
                        reader.Close();
                        System.IO.File.Delete(filename);
                        //Send Data to API
                        var status = "";
                        HttpClient client = new HttpClient();
                        var url = DBConn.HttpString + "/Employee/ImportEmployee";

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
