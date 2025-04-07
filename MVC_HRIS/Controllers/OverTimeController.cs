
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
using System.Net;
using System.ComponentModel.DataAnnotations;
using static MVC_HRIS.Controllers.LeaveController;
using Humanizer;
using System.IO;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace MVC_HRIS.Controllers
{
    public class OverTimeController : Controller
    {
        string status = "";
        private readonly QueryValueService token;
        private readonly AppSettings _appSettings;
        private ApiGlobalModel _global = new ApiGlobalModel();
        DbManager db = new DbManager();
        public readonly QueryValueService token_;
        private IConfiguration _configuration;
        private string apiUrl = "http://";
        public OverTimeController(IOptions<AppSettings> appSettings, QueryValueService _token,
                  IHttpContextAccessor contextAccessor,
                  IConfiguration configuration)
        {
            token_ = _token;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("AppSettings:WebApiURL");
            _appSettings = appSettings.Value;
        }
        public class EmployeeIdFilter
        {
            public string EmployeeNo { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
        }
        public IActionResult OTApproval()
        {
            return PartialView("OTApproval");
        }
        [HttpPost]
        public async Task<IActionResult> GetOverTimeList(EmployeeIdFilter data)
        {
            string result = "";
            var list = new List<OvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/OvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<OvertimeVM>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }
        public IActionResult OverTime()
        {
            return View("_OverTime");
        }
        public IActionResult OTFiling()
        {

            return PartialView("OTFiling");
        }

        public partial class TblOvertimeModelView
        {
            public int Id { get; set; }
            public string? OTNo { get; set; }
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
            public int? CreatedBy { get; set; }
            public string? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public string? DateDeleted { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> SaveOT(TblOvertimeModelView data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/save";

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

        public partial class OvertimeVM
        {
            public int Id { get; set; }
            public string OTNo { get; set; }
            public string EmployeeNo { get; set; }
            public string fullname { get; set; }
            public string Department { get; set; }
            public string Position { get; set; }
            public string PositionLevel { get; set; }
            public string EmployeeType { get; set; }
            public DateTime? Date { get; set; }
            [DataType(DataType.Date)]
            public TimeSpan? StartTime { get; set; }
            [DataType(DataType.Time)]
            public TimeSpan? EndTime { get; set; }
            [DataType(DataType.Time)]
            public DateTime? StartDate { get; set; }
            [DataType(DataType.Date)]
            public DateTime? EndDate { get; set; }
            public decimal? HoursFiled { get; set; }
            public decimal? HoursApproved { get; set; }
            public string? Remarks { get; set; }
            public bool? ConvertToLeave { get; set; }
            public bool? ConvertToOffset { get; set; }
            public DateTime? DateCreated { get; set; }
            public int? LeaveId { get; set; }
            public int? Status { get; set; }
            public string? StatusName { get; set; }
            public int? CreatedBy { get; set; }
            public bool? isDeleted { get; set; }
            public int? DeletedBy { get; set; }
            public DateTime? DateDeleted { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingOvertTimeListSelect(PedingOTFilter data)
        {
            string result = "";
            var list = new List<OvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/OverTime/PendingOvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<OvertimeVM>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(list);
        }
        public class PedingOTFilter
        {
            public string? EmployeeNo { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public int? status { get; set; }
            public int? ManagerId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingOvertTimeList(PedingOTFilter data)
        {
            string result = "";
            var list = new List<OvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/OverTime/PendingOvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<OvertimeVM>>(res);

                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }
            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }

        public class OTUpdateStatus
        {
            public int Id { get; set; }
            public int status { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> updateStatus(OTUpdateStatus data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/updateStatus";

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
        public class multiOTApprovalParamList
        {
            public int? Id { get; set; }
            public string? reason { get; set; }
            public decimal? HoursApproved { get; set; }
        }
        public class multiOTApprovalParam
        {
            public int? Status { get; set; }
            public List<multiOTApprovalParamList> otapproval { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> MultiApproval(multiOTApprovalParam data)
        {
            string res = "";
            try
            {

                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/MultiOTUpdateStatus";

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
        public class CheckedOTRequestListParam
        {
            public string[]? Id { get; set; }
        }
        public partial class TblOvertimeVM
        {
            public string Id { get; set; }
            public string? OTNo { get; set; }
            public string? EmployeeNo { get; set; }
            public DateTime? Date { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? HoursFiled { get; set; }
            public string? HoursApproved { get; set; }
            public string? Remarks { get; set; }
            public string? ConvertToLeave { get; set; }
            public string? ConvertToOffset { get; set; }
            public string? DateCreated { get; set; }
            public int? LeaveId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetCheckedOTRequestList(CheckedOTRequestListParam data)
        {
            string result = "";
            var list = new List<TblOvertimeVM>();
            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/Overtime/CheckedOTRequestList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<TblOvertimeVM>>(res);

                }
            }

            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            return Json(new { draw = 1, data = list, recordFiltered = list?.Count, recordsTotal = list?.Count });
        }

        public IActionResult DownloadHeader()
        {
            var stream = new MemoryStream();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
                ws.Cells["A1"].Value = "Shift Date";
                ws.Cells["B1"].Value = "Start Date";
                ws.Cells["C1"].Value = "End Date";
                ws.Cells["D1"].Value = "Start Time";
                ws.Cells["E1"].Value = "End Time";
                ws.Cells["F1"].Value = "Hours Filed";
                ws.Cells["G1"].Value = "Remarks";
                ws.Cells["H1"].Value = "Convert to Compensatory Time-Off";
                ws.Cells["I1"].Value = "Convert to Offset Time-Off";
                ws.Cells["J1"].Value = "Employee No.";
                ws.Cells["K1"].Value = "UserID";


                // Apply Custom Formatting
                ws.Cells["A:A"].Style.Numberformat.Format = "yyyy-mm-dd"; // Date format with microseconds
                ws.Cells["B:B"].Style.Numberformat.Format = "yyyy-mm-dd";
                ws.Cells["C:C"].Style.Numberformat.Format = "yyyy-mm-dd";
                ws.Cells["D:D"].Style.Numberformat.Format = "hh:mm:ss.0000000"; // Time format with microseconds
                ws.Cells["E:E"].Style.Numberformat.Format = "hh:mm:ss.0000000";
                ws.Cells["H:H"].Style.Numberformat.Format = "BOOLEAN"; // Boolean format
                ws.Cells["I:I"].Style.Numberformat.Format = "BOOLEAN"; // Boolean format
                ws.Cells["F:F"].Style.Numberformat.Format = "0.00";
                ws.Cells["A2"].Value = "yyyy-mm-dd";
                ws.Cells["B2"].Value = "yyyy-mm-dd";
                ws.Cells["C2"].Value = "yyyy-mm-dd";
                ws.Cells["D2"].Value = "hh:mm:ss";
                ws.Cells["E2"].Value = "hh:mm:ss";
                ws.Cells["F2"].Value = "0";
                ws.Cells["H2"].Value = "TRUE";
                ws.Cells["I2"].Value = "FALSE";
                ws.Cells["J2"].Value = HttpContext.Session.GetString("EmployeeID");
                ws.Cells["K2"].Value = HttpContext.Session.GetString("Id");
                // Enforce Boolean (Dropdown with TRUE/FALSE)
                var boolValidationH = ws.DataValidations.AddListValidation("H2:H1000");
                boolValidationH.Formula.Values.Add("TRUE");
                boolValidationH.Formula.Values.Add("FALSE");

                var boolValidationI = ws.DataValidations.AddListValidation("I2:I1000");
                boolValidationI.Formula.Values.Add("TRUE");
                boolValidationI.Formula.Values.Add("FALSE");
                // Apply Bold to Headers
                for (var col = 1; col <= 9; col++)
                {
                    ws.Cells[1, col].Style.Font.Bold = true;
                }
                // Set Formula for Hours Filed (Column F)
                string blank = "''";
                for (int row = 2; row <= 50; row++)
                {
                    ws.Cells[$"F{row}"].Formula = $"=IFERROR(IF(((C{row}+E{row})-(B{row}+D{row}))*24=0,\"\",((C{row}+E{row})-(B{row}+D{row}))*24),\"\")";
                    ws.Cells[$"L{row}"].Formula = $"=IF(A{row}=\"\",\"\",TEXT(A{row},\"yyyy-MM-dd\"))";
                    ws.Cells[$"M{row}"].Formula = $"=IF(B{row}=\"\",\"\",TEXT(B{row},\"yyyy-MM-dd\"))";
                    ws.Cells[$"N{row}"].Formula = $"=IF(C{row}=\"\",\"\",TEXT(C{row},\"yyyy-MM-dd\"))";
                    ws.Cells[$"O{row}"].Formula = $"=IF(D{row}=\"\",\"\",TEXT(D{row},\"hh:mm:ss\"))";
                    ws.Cells[$"P{row}"].Formula = $"=IF(E{row}=\"\",\"\",TEXT(E{row},\"hh:mm:ss\"))";
                }


                // Hide columns J (10), K (11), L (12), M (13), N (14), O (15), P (16)
                for (int col = 10; col <= 16; col++)
                {
                    ws.Column(col).Hidden = true;
                }
                //// Lock Only Column F
                //ws.Cells["F:F"].Style.Locked = true;
                ////ws.Cells["J:J"].Style.Locked = true;
                ////ws.Cells["K:K"].Style.Locked = true;
                // Unlock Other Columns (Users can edit these)
                

                ws.Cells["Q1:Q2"].Style.Font.Italic = true;
                ws.Cells["Q1:Q2"].Style.Font.Color.SetColor(Color.Red);
                ws.Cells["Q1"].Value = "All Fields are required";
                ws.Cells["Q2"].Value = "Please follow the format shown in the first row.";


                ws.Cells.AutoFitColumns();
                int[] lockedColumns = {4, 10, 11, 12, 13, 14, 15, 16 ,17};

                int[] hideColumns = { 10, 11, 12, 13, 14, 15, 16 };
                foreach (int col in lockedColumns)
                {
                    ws.Column(col).Style.Locked = true; // Lock column
                }
                foreach (int col in hideColumns)
                {
                    ws.Column(col).Hidden = true;       // Hide column
                }
                ws.Cells["A:E"].Style.Locked = false;
                ws.Cells["G:I"].Style.Locked = false;
                //// Protect the worksheet to enforce the lock

                ws.Protection.IsProtected = true;
                pck.Save();
            }

            stream.Position = 0;
            string excelName = "Request-Overtime-Template.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        
        [HttpPost]
        public async Task<IActionResult> ExportPendingOvertimeList(PedingOTFilter data)
        {
            string result = "";
            var list = new List<OvertimeVM>();

            try
            {
                HttpClient client = new HttpClient();
                var url = DBConn.HttpString + "/OverTime/PendingOvertTimeList";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token_.GetValue());
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                using (var response = await client.PostAsync(url, content))
                {
                    string res = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<OvertimeVM>>(res);
                }
            }
            catch (Exception ex)
            {
                string status = ex.GetBaseException().ToString();
            }

            
            return Ok(list);
        }

        //public IActionResult ExportOTList(PedingOTFilter data)
        //{
        //    var list =  ExportPendingOvertimeList(data) ;
        //    var stream = new MemoryStream();
        //    // Export to Excel
        //    if (list != null && list.Count > 0)
        //    {
                
        //        using (var pck = new ExcelPackage(stream))
        //        {
        //            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet 1");
        //            // Add headers
        //            ws.Cells[1, 1].Value = "Employee ID";
        //            ws.Cells[1, 2].Value = "Employee Name";
        //            ws.Cells[1, 3].Value = "Overtime Hours";
        //            ws.Cells[1, 4].Value = "Approval Status";
        //            ws.Cells[1, 5].Value = "Date";

        //            // Add data
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                ws.Cells[i + 2, 1].Value = list[i].EmployeeNo;
        //                ws.Cells[i + 2, 2].Value = list[i].fullname;
        //                ws.Cells[i + 2, 3].Value = list[i].HoursApproved;
        //                ws.Cells[i + 2, 4].Value = list[i].StatusName;
        //                ws.Cells[i + 2, 5].Value = list[i].Date;
        //            }

        //            // Auto fit columns for better readability
        //            ws.Cells[ws.Dimension.Address].AutoFitColumns();

        //            // Save the file to memory
        //            pck.Save();

        //            // Return the file as a download
        //            stream.Position = 0;

        //        }
        //    }
        //    stream.Position = 0;
        //    string excelName = "Overtime-List.xlsx";
        //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        //}
    }
}
