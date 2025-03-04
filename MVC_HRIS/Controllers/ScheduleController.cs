using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVC_HRIS.Manager;
using MVC_HRIS.Models;
using MVC_HRIS.Services;
using API_HRIS.Manager;
using API_HRIS.Models;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AOPC_CMSv2.Controllers
{
    public class ScheduleController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public ScheduleController(IOptions<AppSettings> appSettings, QueryValueService _token,
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
            string token = HttpContext.Session.GetString("Bearer");
            if (token == "" || token == null)
            {
                return RedirectToAction("Index", "LogIn");
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetScheduleList()
        {
            string result = "";

            var list = new List<TblScheduleModel>();
            try
            {
                string test = token_.GetValue();
                var url = DBConn.HttpString + "/Schedule/ScheduleList";
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                string response = await client.GetStringAsync(url);
                result = response;

                list = JsonConvert.DeserializeObject<List<TblScheduleModel>>(result);
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        [HttpGet]
        public async Task<JsonResult> GetScheduleListOption()
        {
            string result = "";

            var list = new List<TblScheduleModel>();
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/Schedule/ScheduleList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblScheduleModel> models = JsonConvert.DeserializeObject<List<TblScheduleModel>>(response);
            return new(models);
        }
        [HttpPost]
        public async Task<IActionResult> AddSchedule(TblScheduleModel data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Schedule/saveschedule";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    HttpStatusCode statusCode = response.StatusCode;
                    res = await response.Content.ReadAsStringAsync();

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
