
using MVC_HRIS.Models;
using MVC_HRIS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using static Humanizer.On;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Options;
using System.Text;
using System.IO;
using System;
using System.Data;
using AuthSystem.Manager;
using MVC_HRIS.Models;
using CMS.Models;
using MVC_HRIS.Services;
using MVC_HRIS.Manager;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using API_HRIS.Manager;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using static MVC_HRIS.Controllers.OverTimeController;
namespace AOPC.Controllers
{

    public class LogInController : Controller
    {
        DBMethods dbmet = new DBMethods();
        DbManager db = new DbManager();
        private ApiGlobalModel _global = new ApiGlobalModel();
        private IWebHostEnvironment Environment;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _environment;
        public static string UserId;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        private string status = "";
        private readonly QueryValueService token_val;
        public LogInController( IOptions<AppSettings> appSettings, IWebHostEnvironment _environment,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration, QueryValueService _token)
        {

            token_val = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
            Environment = _environment;

        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(loginCredentials data)
        {
            // Assuming LogIn is a method that returns a status string
            var status = await LogIn(data);
            if (status.Status == "Ok")
            {
                // Get the token value
                //string token = token_val.GetValue();
                //var url = DBConn.HttpString + "/Module/ModuleList";

                //// Use HttpClient inside a `using` block to ensure proper disposal
                //using (HttpClient client = new HttpClient())
                //{
                //    // Set the Authorization header properly, assuming Bearer token
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //    // Make the GET request to the specified URL
                //    string response = await client.GetStringAsync(url);

                //    // Deserialize the response into a list of models
                //    List<TblModulesModel> models = JsonConvert.DeserializeObject<List<TblModulesModel>>(response);
                //    HttpContext.Session.SetString("MyList", JsonConvert.SerializeObject(models));

                //    ViewBag.Modules = models;
                //    // Serialize the list of models and store it in TempData

                //}
                //Remember me token
                var jwtoken = Cryptography.Encrypt(data.username + "odecci2025!" + data.username);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, jwtoken),  // Store the encrypted token (JWT) in the "Name" claim (or a custom claim type)
                    // You can add more claims if necessary (roles, permissions, etc.)
                };

                // Step 3: Create a ClaimsIdentity with the encrypted token as part of the claims
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                var authProperties = new AuthenticationProperties
                {

                    IsPersistent = data.rememberToken ?? false, // Defaults to false if null
                    ExpiresUtc = (data.rememberToken ?? false)
                        ? DateTime.UtcNow.AddDays(30)
                        : DateTime.UtcNow.AddMinutes(10)
                };
                if (data.rememberToken == true) { 
                    await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                    HttpContext.Session.SetString("RememberMe", "1");
                    // Set the custom rememberMeToken cookie
                    HttpContext.Response.Cookies.Append(
                        "MyCookieAuth",        // Cookie name
                        jwtoken,               // Cookie value (encrypted token)
                        new CookieOptions
                        {
                            HttpOnly = true,       // Prevent access to the cookie via JavaScript (security measure)
                            Secure = true,         // Ensure the cookie is sent over HTTPS only
                            Expires = DateTime.UtcNow.AddDays(30)  // Set expiration date
                        });
                }
               
                // Redirect to the dashboard
                if (status.UserType == "2")
                {
                    return Json(new { redirectToUrl = Url.Action("Index", "Dashboard") });
                }
                else if (status.UserType == "3")
                {
                    return Json(new { redirectToUrl = Url.Action("Index", "TimeLogs") });
                }
                else
                {
                    return Json(new { stats = status });
                }

            }
            else
            {
                // Return the status if login failed
                return Json(new { stats = status });
            }
        }
        public class LoginStats
        {
            public string Status { get; set; }
            public string UserType { get; set; }

        }
        public partial class loginCredentials
        {
            public string? username { get; set; }
            public string? password { get; set; }
            public string? ipaddress { get; set; }
            public string? location { get; set; }
            public bool? rememberToken { get; set; }
        }
        public class LoginResult
        {
            public string Status { get; set; }
            public string UserType { get; set; }
        }
        public async Task<LoginResult> LogIn(loginCredentials data)
        {
            var result = new LoginResult();
            try
            {
                //var pass3 = Cryptography.Encrypt("odecciaccounting2025!");
                string sql = $@"SELECT    ODC_HRIS.dbo.tbl_UsersModel.Id, ODC_HRIS.dbo.tbl_UsersModel.Username, ODC_HRIS.dbo.tbl_UsersModel.Password, ODC_HRIS.dbo.tbl_UsersModel.Fullname, ODC_HRIS.dbo.tbl_UsersModel.Fname, ODC_HRIS.dbo.tbl_UsersModel.Lname, 
                                    ODC_HRIS.dbo.tbl_UsersModel.Mname, ODC_HRIS.dbo.tbl_UsersModel.Email, ODC_HRIS.dbo.tbl_UsersModel.Gender, ODC_HRIS.dbo.tbl_UsersModel.EmployeeID, ODC_HRIS.dbo.tbl_UsersModel.JWToken, 
                                    ODC_HRIS.dbo.tbl_UsersModel.FilePath, ODC_HRIS.dbo.tbl_UsersModel.Active as ActiveStatusId, ODC_HRIS.dbo.tbl_UsersModel.Cno, ODC_HRIS.dbo.tbl_UsersModel.Address, ODC_HRIS.dbo.tbl_StatusModel.id AS StatusId, 
                                    ODC_HRIS.dbo.tbl_StatusModel.Status, ODC_HRIS.dbo.tbl_UsersModel.Date_Created, ODC_HRIS.dbo.tbl_UsersModel.Date_Updated, ODC_HRIS.dbo.tbl_UsersModel.Delete_Flag, ODC_HRIS.dbo.tbl_UsersModel.Created_By, 
                                    ODC_HRIS.dbo.tbl_UsersModel.Updated_By, ODC_HRIS.dbo.tbl_UsersModel.Date_Deleted, ODC_HRIS.dbo.tbl_UsersModel.Deleted_By, ODC_HRIS.dbo.tbl_UsersModel.Restored_By, 
                                    ODC_HRIS.dbo.tbl_UsersModel.Date_Restored, ODC_HRIS.dbo.tbl_UsersModel.Department, ODC_HRIS.dbo.tbl_UsersModel.AgreementStatus, ODC_HRIS.dbo.tbl_UsersModel.RememberToken, 
                                    ODC_HRIS.dbo.tbl_SalaryType.SalaryType, ODC_HRIS.dbo.tbl_SalaryType.Rate, ODC_HRIS.dbo.tbl_PayrollType.PayrollType, tbl_UsersModel.UserType, tbl_UserType.UserType as UserTypeName, 
		                            ODC_HRIS.dbo.tbl_EmployeeType.Id as EmployeeTypeId, ODC_HRIS.dbo.tbl_EmployeeType.Title as EmployeeTypeName, ODC_HRIS.dbo.tbl_UsersModel.PositionLevelId as PositionLevel

                                FROM            ODC_HRIS.dbo.tbl_UsersModel INNER JOIN
                                    ODC_HRIS.dbo.tbl_StatusModel ON ODC_HRIS.dbo.tbl_UsersModel.Status = ODC_HRIS.dbo.tbl_StatusModel.id INNER JOIN
                                    ODC_HRIS.dbo.tbl_SalaryType ON ODC_HRIS.dbo.tbl_UsersModel.SalaryType = ODC_HRIS.dbo.tbl_SalaryType.Id INNER JOIN
                                    ODC_HRIS.dbo.tbl_PayrollType ON ODC_HRIS.dbo.tbl_UsersModel.PayrollType = ODC_HRIS.dbo.tbl_PayrollType.Id inner join
		                            ODC_HRIS.dbo.tbl_UserType on ODC_HRIS.dbo.tbl_UsersModel.UserType = ODC_HRIS.dbo.tbl_UserType .Id left join
		                            ODC_HRIS.dbo.tbl_EmployeeType on ODC_HRIS.dbo.tbl_EmployeeType.Id = ODC_HRIS.dbo.tbl_UsersModel.EmployeeType
                     
                            WHERE        ( ODC_HRIS.dbo.tbl_UsersModel.Username = '" + data.username + "' COLLATE Latin1_General_CS_AS) and ( ODC_HRIS.dbo.tbl_UsersModel.Password = '" + Cryptography.Encrypt(data.password) + "' COLLATE Latin1_General_CS_AS) AND ( ODC_HRIS.dbo.tbl_UsersModel.Active = 1)";
                DataTable dt = db.SelectDb(sql).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    string fname = dt.Rows[0]["Fname"].ToString();
                    HttpContext.Session.SetString("Name", dt.Rows[0]["Fname"].ToString() + dt.Rows[0]["Lname"].ToString());
                    //HttpContext.Session.SetString("Position", dt.Rows[0]["PositionName"].ToString());
                    //HttpContext.Session.SetString("UserType", dt.Rows[0]["UserType"].ToString());
                    //HttpContext.Session.SetString("CorporateName", dt.Rows[0]["CorporateName"].ToString());
                    HttpContext.Session.SetString("UserID", dt.Rows[0]["Id"].ToString());
                    HttpContext.Session.SetString("UserName", dt.Rows[0]["Username"].ToString());
                    HttpContext.Session.SetString("EmployeeID", dt.Rows[0]["EmployeeID"].ToString());
                    HttpContext.Session.SetString("UserType", dt.Rows[0]["UserType"].ToString());
                    HttpContext.Session.SetString("UserTypeName", dt.Rows[0]["UserTypeName"].ToString());
                    HttpContext.Session.SetString("EmployeeTypeName", dt.Rows[0]["EmployeeTypeName"].ToString());
                    HttpContext.Session.SetString("PositionLevel", dt.Rows[0]["PositionLevel"].ToString());
                    //HttpContext.Session.SetString("CorporateID", dt.Rows[0]["CorporateID"].ToString());StatusId
                    HttpContext.Session.SetString("Id", dt.Rows[0]["Id"].ToString());
                    //HttpContext.Session.SetString("MembershipName", dt.Rows[0]["MembershipName"].ToString());
                    if (dt.Rows[0]["FilePath"].ToString() == null || dt.Rows[0]["FilePath"].ToString() == string.Empty)
                    {
                        HttpContext.Session.SetString("ImgUrl", "https://www.alfardanoysterprivilegeclub.com/assets/img/defaultavatar.png");
                    }
                    else
                    {
                        HttpContext.Session.SetString("ImgUrl", dt.Rows[0]["FilePath"].ToString());
                    }
                    HttpClient client = new HttpClient();
                    var url = DBConn.HttpString + "/User/login";
                    var token = _global.GenerateToken(data.username, _appSettings.Key.ToString());
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_val.GetValue());
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            
                  
                    using (var response = await client.PostAsync(url, content))
                    {

                        status = await response.Content.ReadAsStringAsync();
                        //List<LoginStats> models = JsonConvert.DeserializeObject<List<LoginStats>>(status);
                        string asdas = JsonConvert.DeserializeObject<LoginStats>(status).Status;
                        string utype = JsonConvert.DeserializeObject<LoginStats>(status).UserType;

                        result.Status = asdas;
                        result.UserType = utype;

                    }
                 
           
                    if (result.Status == "Ok")
                    {
                        //string action = data.Id == 0 ? "Added New" : "Updated";
                        dbmet.InsertAuditTrail("User Id: " + dt.Rows[0]["Id"].ToString() +
                        " Successfully LogIn Name : " + dt.Rows[0]["Fname"].ToString() + dt.Rows[0]["Lname"].ToString(), DateTime.Now.ToString(),
                        " CMS-LogIn",
                        dt.Rows[0]["Fname"].ToString() + dt.Rows[0]["Lname"].ToString(),
                        dt.Rows[0]["Id"].ToString(),
                        "2",
                                dt.Rows[0]["EmployeeID"].ToString());
                        HttpContext.Session.SetString("Bearer", token.ToString());
                        //string test = token_val.GetValue();
                        //token_val.GetValue();
                    }
              
                }
                else
                {
                    //string action = "Deleted";
                    //string action = data.Id == 0 ? "Added New" : "Updated";
                    dbmet.InsertAuditTrail("User Id: Unknown" +
                       " Failed to Log In", DateTime.Now.ToString(),
                       " CMS-LogIn",
                       data.username,
                       "0",
                       "2",
                       "Unknown");
                    result.Status = "Invalid Log IN";
                }    
            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return result;
        

        }
        [HttpGet]
        public IActionResult Check()
        {
            var user = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(user))
            {
                // Optional: re-set the same value to "touch" the session
                HttpContext.Session.SetString("UserName", user);
                return Json(new { isLoggedIn = true });
            }

            return Json(new { isLoggedIn = false });
        }
        public async Task<String> GetUserType(loginCredentials data)
        {
            string result = "";
            string userType = "";
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/User/login";
                var token = _global.GenerateToken(data.username, _appSettings.Key.ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_val.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {

                    userType = await response.Content.ReadAsStringAsync();
                    //List<LoginStats> models = JsonConvert.DeserializeObject<List<LoginStats>>(status);
                    string asdas = JsonConvert.DeserializeObject<LoginStats>(userType).UserType;

                    result = asdas;

                }
            }

            catch (Exception ex)
            {
                status = ex.GetBaseException().ToString();
            }
            return result;


        }
        // Displays the index of the current user.
        public IActionResult Index(loginCredentials data)
        {//update
            string token = HttpContext.Session.GetString("Bearer");
            string userType = HttpContext.Session.GetString("UserType");
            var cookieValue = Request.Cookies["MyCookieAuth"];
            if(cookieValue != null)
            {
                string sql = $@"select * from tbl_UsersModel where RememberToken = '" + cookieValue+"'";
                DataTable dt = db.SelectDb(sql).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    HttpContext.Session.SetString("RememberMe", "1");
                    var pass = dt.Rows[0]["Password"].ToString();
                    var password = Cryptography.Decrypt(pass);
                    data.username = dt.Rows[0]["Username"].ToString();
                    data.password = password;
                    data.rememberToken = true;
                }
                LoginUser(data);
                if (userType != "2")
                {
                    return RedirectToAction("Index", "TimeLogs");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else if(token != null)
            {
                if (userType != "2")
                {
                    return RedirectToAction("Index", "TimeLogs");
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                return View();
            }

            //if (token != "" && token != null)
            //{
            //    if(userType != "2")
            //    {
            //        return RedirectToAction("Index", "TimeLogs");
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", "Dashboard");
            //    }

            //}
            //else
            //{
            //    return View();
            //}
            //return View();
        }
        public IActionResult ForgotPassword()
        {
            return View("~/Views/LogIn/ForgotPassword.cshtml");
        }
        public class UserIdParam
        {
            public int? Id { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> LogoutUser(UserIdParam data)
        {

            string result = "";

            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/User/LogOut";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_val.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    result = res;

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(result);
        }
        public async Task<IActionResult> Logout()
        {

            string rememberMe = HttpContext.Session.GetString("RememberMe") ?? "0";

            HttpContext.Session.SetString("Bearer", "");
            
            //await HttpContext.SignOutAsync("MyCookieAuth");
            // Remove specific cookie
            if(rememberMe == "1")
            {
                Response.Cookies.Delete("MyCookieAuth");
            }
            Response.Cookies.Delete(".AspNetCore.Session");
            //return Json(result);
            return RedirectToAction("Index", "LogIn");
        }
        public class ForgotPasswordParam
        {
            public string email { get; set; }
            public string? vcode { get; set; }
            public string? newPassword { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> SearchAccount(ForgotPasswordParam data)
        {
            string result = "";
            
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/User/SearchAccount";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_val.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    result = res;

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> SearchAccountByVerificationCode(ForgotPasswordParam data)
        {
            string result = "";

            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/User/SearchAccountByVerificationCode";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_val.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    result = res;

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> SaveNewPassword(ForgotPasswordParam data)
        {
            string result = "";

            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/User/SaveNewPassword";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_val.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    result = res;

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(result);
        }
    }
}
