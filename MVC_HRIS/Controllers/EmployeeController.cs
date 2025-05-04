
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
    }
}
