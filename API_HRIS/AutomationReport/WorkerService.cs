using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace API_HRIS.AutomationReport
{
    public class WorkerService : BackgroundService
    {
        //private const int generalDelay = 1 * 10 * 1000; // 10 seconds
        private static readonly TimeSpan generalDelay = TimeSpan.FromDays(30);
        private readonly IServiceScopeFactory _scopeFactory;

        public WorkerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(generalDelay, stoppingToken);
                await DoBackupAsync();
                await OverTimeReport();
            }
        }

        private static Task DoBackupAsync()
        {
            // here i can write logic for taking backup at midnight
            Console.WriteLine("Executing background task");
            

            return Task.FromResult("Done");
        }
        private async Task OverTimeReport()
        {
            
            using var scope = _scopeFactory.CreateScope();
            var PdfGenerator = scope.ServiceProvider.GetRequiredService<PdfGenerator>();

            var _employeeList = PdfGenerator.GetEmployeeListWithJoins();
            if(_employeeList != null)
            {
                for(int i = 0; i< _employeeList.Count; i++)
                {

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Odecci", "info@odecci.com"));
                    message.To.Add(new MailboxAddress("France Samaniego", "france.samaniego@odecci.com"));
                    message.Subject = "Overtime Form["+ _employeeList[i].Fullname + "]";
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
                                        <title>Overtime Status Updated</title>
                                        </head>
                                            <body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
                                                <h1>Hi</h1>
                                            </body>
                                        </html>";
                    bodyBuilder.Attachments.Add("report.pdf", pdfBytes, new ContentType("application", "pdf"));
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
