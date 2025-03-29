
using MVC_HRIS.Models;
using CMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Data;
using X.PagedList;
using X.PagedList.Mvc.Core;
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
using API_HRIS.Manager;
using System.Net;

namespace MVC_HRIS.Controllers
{
    public class PayrollController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public PayrollController(IOptions<AppSettings> appSettings,  QueryValueService _token,
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
            return View();
        }

        public IActionResult Payroll()
        {
            return View("_Payroll");
        }
        public async Task<IActionResult> PayslipAdmin(RequestParamenter data, int page = 1, int pageSize = 5)
        {
            // Retrieve data and map it correctly
            var payslipData = await GetPayslipAdmin(data);

            var result = payslipData
                .Select(p => new TblPayslipVM
                {
                    EmployeeNumber = p.EmployeeNumber,
                    UserId = p.UserId,
                    EmployeeName = p.EmployeeName,
                    GrossPay = p.GrossPay,
                    NetPay = p.NetPay,
                    Tax = p.Tax,
                    SSS = p.SSS,
                    PhilHealth = p.PhilHealth,
                    PagIbig = p.PagIbig,
                    PayDate = p.PayDate,
                    TotalDeductions = p.TotalDeductions,
                    RenderedHours = p.RenderedHours,
                    OvertimePay=p.OvertimePay,
                    OvertimeHours=p.OvertimeHours
                })
                .OrderByDescending(p => p.PayDate)
                .ToList();

            var totalCount = payslipData.Count;

            // Assign result to StaticPagedList correctly
            var viewModel = new PayslipViewModel
            {
                Payslips = new StaticPagedList<TblPayslipVM>(
                    result, page, pageSize, totalCount)
            };

            // Return the partial view with data
            return PartialView("_PayslipAdmin", viewModel);
        }

        public IActionResult Payslip(int employeeid , string datefrom, string dateto)
        {
            var data = new EmployeeFilters
            {
                EmployeeID = employeeid,
                datefrom = datefrom,
                dateto = dateto
            };
            var result = GetPayslip(data).GetAwaiter().GetResult().FirstOrDefault();
            var model = new TblPayslipVM
            {
                RenderedHours=result.RenderedHours,
                EmployeeName=result.EmployeeName,
                UserId = result.UserId,
                EmployeeNumber = result.EmployeeNumber,
                PayslipNumber=result.PayslipNumber,
                JobTitle = result.JobTitle,
                Department = result.Department,
                TaxNumber = result.TaxNumber,
                PayDate = result.PayDate,
                GrossPay = result.GrossPay,
                NetPay = result.NetPay,
                Tax = result.Tax,
                SSS = result.SSS,
                PhilHealth = result.PhilHealth,
                PagIbig = result.PagIbig,
                OtherDeductions = 0,
                TotalDeductions = result.TotalDeductions,
                OvertimeHours = result.OvertimeHours,
                OvertimePay = result.OvertimePay,
            };
            return PartialView("_Payslip", model);
        }
        public class EmployeeFilters
        {
            public int EmployeeID { get; set; }
            public string datefrom { get; set; }
            public string dateto { get; set; }
        }
        public class RequestParamenter
        {
            public int? EmployeeID { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }
        public class EmployeeFiltersAdmin
        {
            public string datefrom { get; set; }
            public string dateto { get; set; }
        }
 
        public async Task<List<TblPayslipVM>> GetPayslipAdmin(RequestParamenter data)
        {
            string result = "";
            var list = new List<TblPayslipVM>();
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Payroll/ComputePayslipAdmin";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblPayslipVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return list;
        }
        public async Task<List<TblPayslipVM>> GetPayslip(EmployeeFilters data)
        {
            var res = new List<TblPayslipVM>();

            try
            {
            
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Payroll/ComputePayslip";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    res = JsonConvert.DeserializeObject<List<TblPayslipVM>>(result);

                }

            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return res;
        }
        public class NotificationPaginateModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }


        }
        public class FilterPayrollType
        {

            public string? PayrollType { get; set; }
            public int page { get; set; }
        }
        [HttpGet]
        public async Task<JsonResult> GetPayrollType()
        {
            
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/PayrollType/PayrollTypeList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblPayrollType> models = JsonConvert.DeserializeObject<List<TblPayrollType>>(response);
            return new(models);
        }
        [HttpGet]
        public async Task<IActionResult> GetPayrollTypeList()
        {
            string test = token_.GetValue();
            var url = DBConn.HttpString + "/PayrollType/PayrollTypeList";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());

            string response = await client.GetStringAsync(url);
            List<TblPayrollType> models = JsonConvert.DeserializeObject<List<TblPayrollType>>(response);
            return Json(new { draw = 1, data = models, recordFiltered = models?.Count, recordsTotal = models?.Count });
        }
        [HttpPost]
        public async Task<IActionResult> SavePayroll(TblPayrollType data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/PayrollType/save";

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
