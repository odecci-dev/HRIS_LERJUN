using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace API_HRIS.AutomationReport
{
    public class WorkerService : BackgroundService
    {
        private const int generalDelay = 1 * 10 * 1000; // 10 seconds
        //private static readonly TimeSpan generalDelay = TimeSpan.FromDays(30);
        private readonly IServiceScopeFactory _scopeFactory;

        public WorkerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        await Task.Delay(generalDelay, stoppingToken);
        //        //await DoBackupAsync();
        //        await OverTimeReport();
        //    }
        //}
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;

                // If today is the 11th and it's a suitable time to run
                if (now.Day == 11 && now.Hour == 12 && now.Minute == 55 || now.Day == 26 && now.Hour == 12 && now.Minute == 55)
                {
                    await OverTimeReport();
                    // Wait a day to avoid running multiple times within the same day
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                else
                {
                    // Wait until the next minute to recheck
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                //await DoBackupAsync();
            }
        }
        private async Task OverTimeReport()
        {
            using var scope = _scopeFactory.CreateScope();
            var PdfGenerator = scope.ServiceProvider.GetRequiredService<PdfGenerator>();
            var _employeeList = PdfGenerator.GetEmployeeListWithJoins();
            if(_employeeList != null)
            {
                DateTime subjecttoday = DateTime.Today;
                DateTime subjectdateFrom = DateTime.Today;
                DateTime subjectdateTo = DateTime.Today;

                if (subjecttoday.Month == 1 && subjecttoday.Day < 26 && subjecttoday.Day > 10)
                {
                    subjectdateFrom = new DateTime((subjecttoday.Year-1), 12, 26);
                    subjectdateTo = new DateTime((subjecttoday.Year), subjecttoday.Month, 11);
                }
                else if (subjecttoday.Month == 1 && subjecttoday.Day < 11)
                {
                    subjectdateFrom = new DateTime((subjecttoday.Year - 1), (subjecttoday.Month - 1), 11);
                    subjectdateTo = new DateTime((subjecttoday.Year - 1), (subjecttoday.Month - 1), 26);
                }
                else if (subjecttoday.Day < 26 && subjecttoday.Day > 10)
                {

                    subjectdateFrom = new DateTime(subjecttoday.Year, (subjecttoday.Month - 1), 26);
                    subjectdateTo = new DateTime(subjecttoday.Year, subjecttoday.Month, 11);
                }
                else if (subjecttoday.Day > 25)
                {
                    subjectdateFrom = new DateTime(subjecttoday.Year, subjecttoday.Month, 11);
                    subjectdateTo = new DateTime(subjecttoday.Year, subjecttoday.Month, 26);
                }
                else
                {
                    subjectdateFrom = new DateTime(subjecttoday.Year, (subjecttoday.Month-1), 11);
                    subjectdateTo = new DateTime(subjecttoday.Year, (subjecttoday.Month-1), 26);
                }
                string subjectformattedDateFrom = subjectdateFrom.ToString("yyyy-MM-dd"); // or your preferred format
                string subjectformattedDateTo = subjectdateTo.ToString("yyyy-MM-dd"); // or your preferred format
                for (int i = 0; i< _employeeList.Count; i++)
                {

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Odecci", "info@odecci.com"));
                    //Email to Employee
                    if (_employeeList[i].Fullname != null || _employeeList[i].Fullname != "" && _employeeList[i].Email != null || _employeeList[i].Email != "")
                    {
                        message.To.Add(new MailboxAddress(_employeeList[i].Fullname, _employeeList[i].Email));
                    }
                    //Email to Manager
                    if (_employeeList[i].Manager != null || _employeeList[i].Manager != "" && _employeeList[i].ManagerEmail != null || _employeeList[i].ManagerEmail != "")
                    {
                        message.To.Add(new MailboxAddress(_employeeList[i].Manager, _employeeList[i].ManagerEmail));
                    }
                    //message.To.Add(new MailboxAddress("France Samaniego", "france.samaniego@odecci.com"));
                    //Email to Payroll
                    message.To.Add(new MailboxAddress("John Alfred Abalos", "john.abalos@odecci.com"));
                    message.To.Add(new MailboxAddress("Odecci Payroll", "payroll@odecci.com"));
                    message.To.Add(new MailboxAddress("Ann Santos", "ann.santos@odecci.com"));
                    message.Subject = "Overtime Request Form - "+ _employeeList[i].Fullname + "["+ subjectformattedDateFrom + " to "+ subjectformattedDateTo+"]";
                    string employeeNo = _employeeList[i].EmployeeId ?? "No EmployeeId";
                    string employeeFullname = _employeeList[i].Fullname ?? "No Fullname";
                    string jobTitle = _employeeList[i].Position ?? "No Position";
                    string immediateSuperVison = _employeeList[i].Manager ?? "No Manager";
                    string department = _employeeList[i].Department ?? "No Department";
                    string rate = _employeeList[i].Rate ?? "0";
                    var pdfBytes = PdfGenerator.GenerateSamplePdf(employeeNo, employeeFullname, jobTitle, immediateSuperVison, department, rate);
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = @"
                                            <html>
                                                <head>
                                                <meta charset='UTF-8' />
                                                <title>Overtime Request</title>
                                                </head>
                                                <body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
                                                <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 30px 0;'>
                                                    <tr>
                                                    <td align='center'>
                                                        <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 0 10px rgba(0,0,0,0.08);'>
            
                                                        <!-- Header -->
                                                        <tr>
                                                            <td style='background-color: #205375; padding: 20px; color: #ffffff; text-align: center; font-size: 20px; font-weight: bold;'>
                                                            Overtime Request
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style='padding: 30px; color: #333333; font-size: 16px; line-height: 1.6;'>
                                                            <p style='margin-top: 0;'>Hello Team,</p>
                                                            <p>Attached is the summary of <strong>"+ _employeeList[i].Fname+" " + _employeeList[i].Lname+ "’s</strong> overtime request form for the period May 26, 2025 to June 10, 2025 for your reference.</p>"
                                                            +"<p>Thank you</p>"
                                                            +"</td>"
                                                        +"</tr>"
                                                        +"<tr>"
                                                            +"<td style='background-color: #f0f0f0; text-align: center; padding: 15px; font-size: 12px; color: #777777;'>"
                                                            +"&copy; 2025 Odecci Solution Inc. All rights reserved."
                                                            +"</td>"
                                                        +"</tr>"

                                                        +"</table>"
                                                    +"</td>"
                                                    +"</tr>"
                                                +"</table>"
                                                +"</body>"
                                            +"</html>";

                    DateTime today = DateTime.Today;
                    string formattedDate = today.ToString("yyyyMMdd");
                    bodyBuilder.Attachments.Add("ODC-"+ formattedDate + "-"+ _employeeList[i].Fname +" "+ _employeeList[i].Lname+ "-Overtime.pdf", pdfBytes, new ContentType("application", "pdf"));
                    message.Body = bodyBuilder.ToMessageBody();
                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync("info@odecci.com", "Roq30573");
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }

                    Console.WriteLine("Email sent");
                }

            }
        }
    }
}
