using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net;
using System.Reflection;
using static API_HRIS.ApplicationModel.EntityModels;
using static System.Net.Mime.MediaTypeNames;
// Somewhere in your async method
//using var httpClient = new HttpClient();

//// Replace this with your actual image URL
//var imageUrl = "https://eportal.odeccisolutions.com/img/odecciLogo.png";

//// Download image as a stream
//await using var stream = await httpClient.GetStreamAsync(imageUrl);

namespace API_HRIS.AutomationReport
{
    public class PdfGenerator
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public PdfGenerator(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            this.dbmet = _dbmet;
        }
        public class OTEmployeeListViewModel
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
            public string? Manager { get; set; }
            public string? ManagerEmail { get; set; }
            public string? Rate { get; set; }
            public string? DaysInMonth { get; set; }
            
        }
        public List<OTEmployeeListViewModel> GetEmployeeListWithJoins()
        {
            try
            {
                var ot = _context.TblOvertimeModel.ToList();

                DateTime today = DateTime.Today;
                DateTime dateFrom = DateTime.Today;
                DateTime dateTo = DateTime.Today;
                if (today.Month == 1 && today.Day < 26 && today.Day > 10)
                {
                    dateFrom = new DateTime((today.Year - 1), 12, 26);
                    dateTo = new DateTime((today.Year), today.Month, 11);
                }
                else if (today.Month == 1 && today.Day < 11)
                {
                    dateFrom = new DateTime((today.Year - 1), (today.Month - 1), 11);
                    dateTo = new DateTime((today.Year - 1), (today.Month - 1), 26);
                }
                else if (today.Day < 26 && today.Day > 10)
                {
                    dateFrom = new DateTime(today.Year, today.Month - 1, 26);
                    dateTo = new DateTime(today.Year, today.Month, 11);
                }
                else if (today.Day > 25)
                {
                    dateFrom = new DateTime(today.Year, (today.Month), 11);
                    dateTo = new DateTime(today.Year, today.Month, 26);
                }
                else
                {
                    dateFrom = new DateTime(today.Year, (today.Month - 1), 11);
                    dateTo = new DateTime(today.Year, today.Month - 1, 26);
                }
                ot = ot
                        .Where(x => x.Date >= dateFrom && x.Date <= dateTo && x.isDeleted == false)
                        .ToList();

                var employeelistdb = _context.GetEmployees()
                    .Where(a => a.Delete_Flag == false)
                    .OrderByDescending(a => a.Id)
                    .ToList();

                var positiondb = _context.TblPositionModels
                    .Where(a => a.DeleteFlag == 0)
                    .OrderByDescending(a => a.Id)
                    .ToList();

                var departmentdb = _context.TblDeparmentModels
                    .Where(a => a.DeleteFlag == 0)
                    .OrderByDescending(a => a.Id)
                    .ToList();

                var employeetypedb = _context.TblEmployeeTypes
                    .Where(a => a.DeleteFlag == 0)
                    .OrderByDescending(a => a.Id)
                    .ToList();

                var positionleveldb = _context.TblPositionLevelModels
                    .Where(a => a.DeleteFlag == false)
                    .OrderByDescending(a => a.Id)
                    .ToList();
                var result = (from employee in employeelistdb

                              join position in positiondb on employee.Position 
                              equals position.Id into empdetails
                              from position in empdetails.DefaultIfEmpty()

                              join manager in employeelistdb on employee.ManagerId
                              equals manager.Id into managerdetails
                              from manager in managerdetails.DefaultIfEmpty()

                              join department in departmentdb on employee.Department 
                              equals department.Id into departmentgroup
                              from department in departmentgroup.DefaultIfEmpty()

                              join etype in employeetypedb on employee.EmployeeType 
                              equals etype.Id into etypegroup
                              from etype in etypegroup.DefaultIfEmpty()

                              join plevel in positionleveldb on employee.PositionLevelId 
                              equals plevel.Id into plevelgroup
                              from plevel in plevelgroup.DefaultIfEmpty()

                              join overtime in ot on employee.EmployeeID 
                              equals overtime.EmployeeNo into otgroup
                              from overtime in otgroup.DefaultIfEmpty()

                              where overtime?.Id != null

                              select new OTEmployeeListViewModel
                              {
                                  Id = employee.Id,
                                  EmployeeId = employee.EmployeeID ?? "",
                                  Fname = employee.Fname,
                                  Lname = employee.Lname,
                                  Fullname = employee.Fullname,
                                  FilePath = employee.FilePath,
                                  Email = employee.Email,
                                  Gender = employee.Gender,
                                  Position = position != null ? position.Name : "No Position",
                                  Manager = manager != null ? manager.Fullname : "No Supervisor Assigned",
                                  ManagerEmail = manager != null ? manager.Email : "No Supervisor Assigned",
                                  Department = department != null ? department.DepartmentName : "No Department",
                                  EmployeeType = etype != null ? etype.Title : "No Employee Type",
                                  PositionLevel = plevel != null ? plevel.Level : "No Position Level",
                                  Rate = employee != null ? employee.Rate.ToString() : "0",
                              })
                              .GroupBy(x => x.EmployeeId) // or x.Id
                              .Select(g => g.First())
                              .ToList();


                return result.ToList();
            }
            catch (Exception ex)
            {
                // You can choose to log or rethrow the exception
                throw new Exception("Failed to get employee list", ex);
            }
        }

        public byte[] GenerateSamplePdf(string employeeNo, string employeeFullname, string jobTitle, string immediateSuperVison, string department, string rate)
        {
            var ot = _context.TblOvertimeModel.ToList();

            DateTime today = DateTime.Today;
            DateTime dateFrom = DateTime.Today;
            DateTime dateTo = DateTime.Today;
            if (today.Month == 1 && today.Day < 26 && today.Day > 10)
            {
                dateFrom = new DateTime((today.Year - 1), 12, 26);
                dateTo = new DateTime((today.Year), today.Month, 11);
            }
            else if (today.Month == 1 && today.Day < 11)
            {
                dateFrom = new DateTime((today.Year - 1), (today.Month - 1), 11);
                dateTo = new DateTime((today.Year - 1), (today.Month - 1), 26);
            }
            else if (today.Day < 26 && today.Day > 10)
            {
                dateFrom = new DateTime(today.Year, today.Month - 1, 26);
                dateTo = new DateTime(today.Year, today.Month, 11);
            }
            else if (today.Day > 25)
            {
                dateFrom = new DateTime(today.Year, (today.Month), 11);
                dateTo = new DateTime(today.Year, today.Month, 26);
            }
            else
            {
                dateFrom = new DateTime(today.Year, (today.Month - 1), 11);
                dateTo = new DateTime(today.Year, today.Month - 1, 26);
            }
            ot = ot.Where(x => x.Date >= dateFrom && x.Date <= dateTo && x.EmployeeNo == employeeNo && x.isDeleted == false).ToList();
            int count = ot?.Count() ?? 0;
            var OvertimeHours = ot.Where(x => x.Date >= dateFrom && x.Date <= dateTo && x.EmployeeNo == employeeNo && x.isDeleted == false).ToList();
            int ApprovedOTCount = OvertimeHours.Count;
            decimal ApprovedHours = 0;
            string ApprovedTotalHour = "";
            decimal PendingHours = 0;
            string PendingTotalHour = "";
            decimal RejectedHours = 0;
            string RejectedTotalHour = "";
            if (ApprovedOTCount != 0)
            {
                for (int i = 0; i < OvertimeHours.Count; i++)
                {
                    string hf = "";
                    if (OvertimeHours[i].HoursFiled != null)
                    {
                        hf = OvertimeHours[i].HoursFiled.ToString();
                        if(OvertimeHours[i].Status == 5)
                        {
                            if (decimal.TryParse(hf, out decimal hoursApproved))
                            {
                                ApprovedHours = (ApprovedHours + hoursApproved);
                            }
                        }
                        else if (OvertimeHours[i].Status == 1004)
                        {
                            if (decimal.TryParse(hf, out decimal hoursPending))
                            {
                                PendingHours = (PendingHours + hoursPending);
                            }
                        }
                        else
                        {
                            if (decimal.TryParse(hf, out decimal hoursRejected))
                            {
                                RejectedHours = (RejectedHours + hoursRejected);
                            }
                        }
                    }
                }
                ApprovedTotalHour = ApprovedHours.ToString() == null ? "0" : ApprovedHours.ToString();
                PendingTotalHour = PendingHours.ToString() == null ? "0" : PendingHours.ToString();
                RejectedTotalHour = RejectedHours.ToString() == null ? "0" : RejectedHours.ToString();
            }
            
            string formattedDateToday = today.ToString("yyyy-MM-dd"); // or your preferred format
            string formattedDateTo = dateTo.ToString("yyyy-MM-dd"); // or your preferred format
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    // 3) a stream
                    //using var stream = new FileStream("AutomationReport/odecciLogo.png", FileMode.Open);
                    using var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData("https://eportal.odeccisolutions.com/img/odecciLogo.png");
                    using var stream = new MemoryStream(imageBytes);
                    //page.Header().AlignMiddle().Column(column =>
                    //{
                    //    column.Item().Row(row =>
                    //    {
                    //        row.RelativeItem().Text("OVERTIME REQUEST FORM")
                    //        .AlignLeft();
                    //        row.ConstantItem(96).AlignRight().Element(x =>
                    //        {
                    //            x.Image(stream);
                    //        });
                    //    });
                    //});
                    page.Header().AlignMiddle().Row(row =>
                    {
                        row.RelativeItem().AlignLeft().AlignBottom().Text("OVERTIME REQUEST FORM")
                            .FontSize(10).Bold();

                        row.ConstantItem(96).AlignRight().Image(stream);

                    });
                    page.Content().Column(column =>
                    {
                        column.Spacing(15);

                        column.Item().Text("No one may be paid for overtime unless this form has been completed in advance of the overtime work. Overtime is paid only when 8 hours have been worked within a day.")
                        .FontSize(8);
                        column.Spacing(15);
                        column.Item().Table(table =>
                        {
                            // Define 3 columns with equal width
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Column 1
                                columns.RelativeColumn(); // Column 2
                                columns.RelativeColumn(); // Column 3
                                columns.RelativeColumn(); // Column 3
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle)
                                .Text("EMPLOYEE NAME")
                                .FontSize(10)
                                .Bold();
                                header.Cell().Element(HeaderCellStyle)
                                .Text("JOB TITLE")
                                .FontSize(10)
                                .Bold();
                                header.Cell().Element(HeaderCellStyle)
                                .Text("EMPLOYEE ID")
                                .FontSize(10)
                                .Bold();
                                header.Cell().Element(HeaderCellStyle)
                                .Text("DATE FORM COMPLETED")
                                .FontSize(10)
                                .Bold();
                            });

                            // Body rows
                            table.Cell().Element(CellStyle)
                            .Text(employeeFullname)
                            .FontSize(10);
                            table.Cell().Element(CellStyle)
                            .Text(jobTitle)
                            .FontSize(10);
                            table.Cell().Element(CellStyle)
                            .Text(employeeNo)
                            .FontSize(10);
                            table.Cell().Element(CellStyle)
                            .Text(formattedDateToday)
                            .FontSize(10);

                            // Styling helper
                            IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Padding(2)
                                    .AlignLeft()
                                    ;
                            }
                            // Styling helper
                            IContainer HeaderCellStyle(IContainer container)
                            {
                                return container
                                    .Border(0)
                                    .Padding(0)
                                    .AlignLeft()
                                    ;
                            }
                        });
                        column.Item().Table(table =>
                        {
                            // Define 3 columns with equal width
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Column 1
                                columns.RelativeColumn(); // Column 2
                                columns.RelativeColumn(); // Column 3
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCellStyle)
                                .Text("IMMEDIATE SUPERVISOR")
                                .FontSize(10)
                                .Bold();
                                header.Cell().Element(HeaderCellStyle)
                                .Text("DEPARTMENT")
                                .FontSize(10)
                                .Bold();
                                header.Cell().Element(HeaderCellStyle)
                                .Text("HOURLY RATE OF PAY")
                                .FontSize(10)
                                .Bold();
                            });

                            // Body rows
                            table.Cell().Element(CellStyle)
                            .Text(immediateSuperVison)
                            .FontSize(10);
                            table.Cell().Element(CellStyle)
                            .Text(department)
                            .FontSize(10);
                            table.Cell().Element(CellStyle)
                            .Text(rate)
                            .FontSize(10);

                            // Styling helper
                            IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Padding(2)
                                    .AlignLeft()
                                    ;
                            }
                            // Styling helper
                            IContainer HeaderCellStyle(IContainer container)
                            {
                                return container
                                    .Border(0)
                                    .Padding(0)
                                    .AlignLeft()
                                    ;
                            }
                        });
                        column.Item().Table(table =>
                        {
                            // Define 3 columns with equal width
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Column 1
                                columns.RelativeColumn(); // Column 2
                                columns.RelativeColumn(); // Column 3
                                columns.RelativeColumn(); // Column 4
                                columns.RelativeColumn(); // Column 5
                                columns.RelativeColumn(); // Column 5
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().ColumnSpan(2).Element(HeaderCellStyle)
                                .Text("DATE OF OVERTIME WORK")
                                .FontSize(10)
                                .Bold();
                                header.Cell().ColumnSpan(2).Element(HeaderCellStyle)
                                .Text("TIME OF OVERTIME WORK")
                                .FontSize(10)
                                .Bold();
                                header.Cell().Element(HeaderCellStyle)
                                .Text("APPROVAL")
                                .FontSize(10)
                                .Bold();
                            });

                            // Body rows
                            table.Cell().Element(CellStyleTitle)
                            .Text("START DATE")
                            .FontSize(10)
                            .Bold();
                            table.Cell().Element(CellStyleTitle)
                            .Text("END DATE")
                            .FontSize(10)
                            .Bold();
                            table.Cell().Element(CellStyleTitle)
                            .Text("START TIME")
                            .FontSize(10)
                            .Bold();
                            table.Cell().Element(CellStyleTitle)
                            .Text("END TIME")
                            .FontSize(10)
                            .Bold();
                            table.Cell().Element(CellStyleTitle)
                            .Text("Hours Filed")
                            .FontSize(10)
                            .Bold();
                            table.Cell().Element(CellStyleTitle)
                            .Text("STATUS")
                            .FontSize(10)
                            .Bold();
                            if (count != 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    
                                    string startTimeString = ot[i].StartTime.ToString();
                                    string formattedStartTime;
                                    if (DateTime.TryParse(startTimeString, out DateTime startTime))
                                    {
                                        formattedStartTime = startTime.ToString("hh:mm tt");  // e.g. "02:30 PM"
                                                                                                // Use formattedTime
                                    }
                                    else
                                    {
                                        // Handle invalid time string if needed
                                        formattedStartTime = "";
                                    }
                                    string endTimeString = ot[i].EndTime.ToString();
                                    string formattedEndTime;
                                    if (DateTime.TryParse(endTimeString, out DateTime endTime))
                                    {
                                        formattedEndTime = endTime.ToString("hh:mm tt");  // e.g. "02:30 PM"
                                                                                              // Use formattedTime
                                    }
                                    else
                                    {
                                        // Handle invalid time string if needed
                                        formattedEndTime = "";
                                    }
                                    string status = "";
                                    if(ot[i].Status == 5)
                                    {
                                        status = "APPROVED";
                                    }
                                    else if (ot[i].Status == 1004)
                                    {
                                        status = "PENDING";
                                    }
                                    else if (ot[i].Status == 1005)
                                    {
                                        status = "REJECTED";
                                    }
                                    // Body rows
                                    table.Cell().Element(CellStyle)
                                    .Text(ot[i].StartDate.ToString().Split(' ')[0])
                                    .FontSize(10);
                                    table.Cell().Element(CellStyle)
                                    .Text(ot[i].EndDate.ToString().Split(' ')[0])
                                    .FontSize(10);
                                    table.Cell().Element(CellStyle)
                                    .Text(formattedStartTime)
                                    .FontSize(10);
                                    table.Cell().Element(CellStyle)
                                    .Text(formattedEndTime)
                                    .FontSize(10);
                                    table.Cell().Element(CellStyle)
                                    .Text(ot[i].HoursFiled.ToString().Split(' ')[0])
                                    .FontSize(10);
                                    table.Cell().Element(CellStyle)
                                    .Text(status)
                                    .FontSize(10);
                                }
                            }


                            // Styling helper
                            IContainer CellStyleTitle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .AlignCenter()
                                    ;
                            }
                            IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Padding(2)
                                    .AlignCenter()
                                    ;
                            }
                            // Styling helper
                            IContainer HeaderCellStyle(IContainer container)
                            {
                                return container
                                    .Border(0)
                                    .Padding(0)
                                    .AlignLeft()
                                    ;
                            }
                        });
                        column.Spacing(15);
                        column.Item().Table(table =>
                        {
                            // Define 3 columns with equal width
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Column 1
                                columns.RelativeColumn(); // Column 2
                                columns.RelativeColumn(); // Column 3
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().ColumnSpan(2).Element(CellStyle)
                                .Text("APPROVED NUMBER OF OVERTIME HOURS")
                                .FontSize(10)
                                .Bold();
                                header.Cell().ColumnSpan(1).Element(CellStyleTitle)
                                .Text(ApprovedTotalHour)
                                .FontSize(10)
                                .Bold();

                                header.Cell().ColumnSpan(2).Element(CellStyle)
                                .Text("PENDING NUMBER OF OVERTIME HOURS")
                                .FontSize(10)
                                .Bold();
                                header.Cell().ColumnSpan(1).Element(CellStyleTitle)
                                .Text(PendingTotalHour)
                                .FontSize(10)
                                .Bold();

                                header.Cell().ColumnSpan(2).Element(CellStyle)
                                .Text("REJECTED NUMBER OF OVERTIME HOURS")
                                .FontSize(10)
                                .Bold();
                                header.Cell().ColumnSpan(1).Element(CellStyleTitle)
                                .Text(RejectedTotalHour)
                                .FontSize(10)
                                .Bold();
                            });



                            // Styling helper
                            IContainer CellStyleTitle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .AlignRight()
                                    ;
                            }
                            IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Padding(2)
                                    .AlignLeft()
                                    ;
                            }
                            // Styling helper
                            IContainer HeaderCellStyle(IContainer container)
                            {
                                return container
                                    .Border(0)
                                    .Padding(0)
                                    .AlignLeft()
                                    ;
                            }
                        });
                        column.Item().Text("Please provide an explanation of the work that requires more than 40 hours/week to complete.")
                        .FontSize(8);
                        column.Item().Table(table =>
                        {
                            // Define 3 columns with equal width
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Column 1
                            });

                            // Header row
                            table.Header(header =>
                            {
                                
                                string remarks = "";
                                if (count != 0)
                                {
                                    string formattedDateReason = "";
                                    for (int i = 0; i < count; i++)
                                    {
                                        //DateTime date = new DateTime(ot[i].Date.ToString());
                                        //string formattedDate = today.ToString("yyyy-MM-dd"); // or your preferred format

                                        string date = ot[i].Date.ToString();
                                        if (DateTime.TryParse(date, out DateTime Date))
                                        {
                                            formattedDateReason = Date.ToString("yyyy/MM/dd");
                                        }
                                        remarks += formattedDateReason + " - ";
                                        remarks += ot[i].Remarks+"; ";
                                        remarks += Environment.NewLine;
                                    }
                                }
                                header.Cell().Element(CellStyle)
                                     .Text(remarks)
                                     .FontSize(10);
                            });
                            IContainer CellStyleTitle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(2)
                                    .AlignCenter()
                                    ;
                            }
                            IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Padding(2)
                                    .AlignLeft()
                                    ;
                            }
                        });

                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("For any overtime related concerns or queries please contact your Immedate Supervisor/Representative.");
                        x.Span(DateTime.Now.ToShortDateString());
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}

