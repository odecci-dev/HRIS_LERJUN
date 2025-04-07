
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
using System.ComponentModel.DataAnnotations;

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
            public string? EmployeeNo { get; set; }
            public string? ManagerId { get; set; }
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

        public partial class TblLeaveRequestModelView
        {
            public int Id { get; set; }
            public string? LeaveRequestNo { get; set; }
            public string? EmployeeNo { get; set; }
            public string? ApprovalReason { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public decimal? DaysFiled { get; set; }
            public int? LeaveTypeId { get; set; }
            public string? Reason { get; set; }
            public DateTime? DateCreated { get; set; }
            public int? Status { get; set; }
            public string? StatusName { get; set; }
            public int? CreatedBy { get; set; }
            public bool? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public DateTime? DateDeleted { get; set; }
            public int? UpdatedBy { get; set; }
            public DateTime? DateUpdated { get; set; }
            public string? Fullname { get; set; }
            public string? Department { get; set; }
            public string? Position { get; set; }
            public string? PositionLevel { get; set; }
            public string? EmployeeType { get; set; }

        }
        public class PendingLeaveRequestListParam
        {
            public string? EmployeeNo { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public int? status { get; set; }
            public int? ManagerId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingLeaveRequestList(PendingLeaveRequestListParam data)
        {
            string result = "";
            var list = new List<TblLeaveRequestModelView>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Leave/PendingLeaveRequestList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblLeaveRequestModelView>>(res);

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

        public IActionResult DownloadHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Date";
                ws.Cells["B1"].Value = "Start Date";
                ws.Cells["C1"].Value = "End Date";
                ws.Cells["D1"].Value = "Days Filed";
                ws.Cells["E1"].Value = "Remarks";
                ws.Cells["F1"].Value = "Leave Type";
                ws.Cells["J1"].Value = "Employee No.";
                ws.Cells["K1"].Value = "UserID";


                // Apply Custom Formatting
                ws.Cells["A:A"].Style.Numberformat.Format = "yyyy-mm-dd"; // Date format with microseconds
                ws.Cells["B:B"].Style.Numberformat.Format = "yyyy-mm-dd";
                ws.Cells["C:C"].Style.Numberformat.Format = "yyyy-mm-dd";
                ws.Cells["D:D"].Style.Numberformat.Format = "0.00";
                ws.Cells["A2"].Value = "yyyy-mm-dd";
                ws.Cells["B2"].Value = "yyyy-mm-dd";
                ws.Cells["C2"].Value = "yyyy-mm-dd";
                ws.Cells["D2"].Value = "0";
                ws.Cells["J2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["K2"].Value = HttpContext.Session.GetString("Id");
                // Apply Bold to Headers
                for (var col = 1; col <= 9; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }
                // Set Formula for Hours Filed (Column F)
                string blank = "''";
                for (int row = 2; row <= 50; row++)
                {
                    ws.Cells[$"D{row}"].Formula = $"=IFERROR(IF(((C{row})-(B{row}))=0,\"\",((C{row})-(B{row}))),\"\")";
                    ws.Cells[$"L{row}"].Formula = $"=IF(A{row}=\"\",\"\",TEXT(A{row},\"yyyy-MM-dd\"))";
                    ws.Cells[$"M{row}"].Formula = $"=IF(B{row}=\"\",\"\",TEXT(B{row},\"yyyy-MM-dd\"))";
                    ws.Cells[$"N{row}"].Formula = $"=IF(C{row}=\"\",\"\",TEXT(C{row},\"yyyy-MM-dd\"))";
                }
                // Hide columns J (10), K (11), L (12), M (13), N (14), O (15), P (16)
                for (int col = 10; col <= 16; col++)
                {
                    ws.Column(col).Hidden = true;
                }
                ws.Cells["Q1:Q2"].Style.Font.Italic = true;
                ws.Cells["Q1:Q2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["Q1"].Value = "All Fields are required";
                ws.Cells["Q2"].Value = "Please follow the format shown in the first row.";
                string sqlLeaveType = $@"SELECT [Id], [Name] FROM [TblLeaveTypeModel]";
                var dataSet = db.SelectDb(sqlLeaveType);

                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    return BadRequest("No leave types found in the database.");
                }

                DataTable dt = dataSet.Tables[0];
                int ctr = 2;
                var validation = ws.DataValidations.AddListValidation("F2:F1000");
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
                    ws.Cells["G" + i].Formula = "=IFERROR(VLOOKUP(F" + i + ",R2:S8,2,FALSE),0)";
                }
                ws.Cells.AutoFitColumns();
                int[] lockedColumns = { 3, 6, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

                int[] hideColumns = { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19 };
                foreach (int col in lockedColumns)
                {
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true;       // Hide column
                }
                ws.Cells["A:C"].Style.Locked = false;
                ws.Cells["E:F"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock

                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Request-Leave-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
