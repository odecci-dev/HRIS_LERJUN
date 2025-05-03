
using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Drawing.Printing;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using API_HRIS.ApplicationModel;
using System.Text;
using static API_HRIS.Manager.DBMethods;
using System.Xml.Linq;
using System;
using System.Text.Json;
using static API_HRIS.ApplicationModel.EntityModels;
using HonkSharp.Fluency;
using MimeKit;
using MailKit.Net.Smtp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Humanizer;
using static AngouriMath.Entity;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Web.Http.Services;
using System.Linq;
using System.Diagnostics.Metrics;
using static API_HRIS.Controllers.DepartmentController;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        private readonly DBMethods dbmet;
        DbManager db = new DbManager();

        string msg_result;
        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public EmployeeController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }
        [HttpPost]
        public async Task<IActionResult> saveEType(TblEmployeeTypeModel data)
        {
            string status = "";
            if (_context.TblScheduleModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblEmployeeTypes'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblEmployeeTypes?.Any(eType => eType.Title == data.Title)).GetValueOrDefault();
            var existingEType = _context.TblEmployeeTypes?.Where(a => a.Id == data.Id).FirstOrDefault();
            if (data.Title == null)
            {
                string query = $@"UPDATE [tbl_EmployeeType]
                                SET DeleteFlag = 1,
                                    DateDeleted = GETDATE() WHERE Id ='" + data.Id + "'";
                db.AUIDB_WithParam(query);

                status = "Schedule successfully Deleted";
                return Ok(status);
            }
            try
            {
                if (data.Id == null || data.Id == 0)
                {

                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    data.DateCreated = DateTime.Now;
                    data.DateUpdated = null;
                    data.DateDeleted = null;
                    data.DeleteFlag = 0;
                    _context.TblEmployeeTypes.Add(data);
                    await _context.SaveChangesAsync();
                    status = "Successfully Save!";
                }
                else
                {
                    if (data.Title != "" || data.Title != null)
                    {
                        data.DateCreated = existingEType?.DateCreated;
                        var eType = _context.TblEmployeeTypes.SingleOrDefault(a => a.Id == data.Id);
                        eType.Id = data.Id;
                        eType.Title = data.Title;
                        eType.Description = data.Description;
                        eType.DeleteFlag = 0;
                        eType.DateUpdated = DateTime.Now;

                        await _context.SaveChangesAsync();
                        status = "Successfully Update!";
                    }

                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpGet]
        public async Task<IActionResult> eTypeList()
        {
            var result = (dynamic)null;
            try
            {
                result = _context.TblEmployeeTypes.Where(a => a.DeleteFlag == 0).ToList();
            }
            catch
            {
                return BadRequest("ERROR");
            }


            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeFilteredById(IdFilter data)
        {
            try
            {
                //var result = _context.TblUsersModels.Where(a => a.Id == data.Id).OrderByDescending(a => a.DateCreated).ThenByDescending(a => a.DateUpdated).ToList();
                var result = _context.GetEmployees().Where(a => a.Id == data.Id)
                    .Select(a => new {
                        a.Id,
                        a.Username,
                        a.Fullname,
                        a.Fname,
                        a.Lname,
                        a.Mname,
                        a.Suffix,
                        a.Email,
                        a.Gender,
                        Status = a.StatusId,
                        EmployeeId = a.EmployeeID,
                        a.FilePath,
                        a.Cno,
                        a.Address,
                        a.Department,
                        UserType = a.UserTypeId,
                        a.EmployeeType,
                        SalaryType = a.SalaryTypeId,
                        a.Rate,
                        a.DaysInMonth,
                        PayrollType = a.PayrollTypeId,
                        a.DateStarted,
                        a.Position,
                        a.PositionLevelId,
                        a.ManagerId,
                        a.SSS_Number,
                        a.PagIbig_MID_Number,
                        a.PhilHealth_Number,
                        a.Tax_Identification_Number,
                        // skip IsActive, RoleId, etc. if they're problematic
                    })
                                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }

        }
        [HttpPost]
        public async Task<IActionResult> ECEmployeeFilteredById(IdFilter data)
        {
            try
            {
                var result = _context.TblEmergencyContactsModel.Where(a => a.UserId == data.Id).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }

        }
        public class EmployeeRegistrationModel
        {
            public string? EmployeeId { get; set; }
            public string? Password { get; set; }
            public string? Email { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> EmployeeRegistration(EmployeeRegistrationModel data)
        {

            var pass = Cryptography.Encrypt(data.Password);
            string status = "";
            //var result = _context.TblUsersModels.Where(a=>a.EmployeeId == data.EmployeeId && a.Email == data.Email && a.Status != 6 ).FirstOrDefault();
            string userSql = $@"SELECT Id, Email, EmployeeID, Status FROM tbl_UsersModel WITH(NOLOCK) WHERE Email = '" + data.Email + "' AND EmployeeID = '" + data.EmployeeId + "' AND Status != 6";
            DataTable table = db.SelectDb(userSql).Tables[0];
            if (table.Rows.Count == 0)
            {

                return NotFound();
            }
            //if (result == null)
            //{
            //    return NotFound();
            //}
            else
            {
                foreach (DataRow dr in table.Rows)
                {
                    status = dr["status"].ToString();
                }

                if (status == "2")
                {
                    return BadRequest("Account is not yet registered or InActive Please Contact the Administrator");
                }
                else
                {
                    //result.Password = pass;
                    //result.Username = data.EmployeeId;
                    //result.Status =6;
                    //// Save changes to the database
                    //_context.Entry(result).State = EntityState.Modified;
                    //await _context.SaveChangesAsync();
                    string query = $@"UPDATE tbl_UsersModel
                                    SET Password = '" + pass + "',Status = '6'"
                                    + "WHERE EmployeeID = '" + data.EmployeeId + "'";
                    db.AUIDB_WithParam(query);

                    //result.Status = "OTP Matched!";
                    //return Ok(result);
                    return Ok();
                }

            }


        }
        [HttpGet]
        public async Task<IActionResult> StatusTypeList()
        {
            return Ok(_context.TblStatusModels.ToList());
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
            public int? ManagerId { get; set; }
            public string? Rate { get; set; }
            public string? DaysInMonth { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> EmployeePaginationList(FilterEmployee data)
        {

            try
            {

                //var Member = _context.GetEmployees().ToList().OrderByDescending(a => a.Id);
                //string status = "Employee successfully viewed";
                //dbmet.InsertAuditTrail("View All Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", "User", "0");
                //return Ok(Member);

                var employeelistdb = _context.GetEmployees()
                    .Where(a => a.Delete_Flag == false)
                    .ToList()
                    .OrderByDescending(a => a.Id);
                var positiondb = _context.TblPositionModels
                    .Where(a => a.DeleteFlag == 0)
                    .OrderByDescending(a => a.Id)
                    .ToList();
                var result = from employee in employeelistdb
                             join position in positiondb
                             on employee.Position equals position.Id into empdetails
                             from ed in empdetails.DefaultIfEmpty()
                             select new EmployeeListViewModel
                             {
                                 Id = employee.Id,
                                 EmployeeId = employee.EmployeeID ?? "",
                                 Fname = employee.Fname,
                                 Lname = employee.Lname,
                                 Fullname = employee.Fullname,
                                 FilePath = employee.FilePath,
                                 Email = employee.Email,
                                 Gender = employee.Gender,
                                 Position = ed != null ? ed.Name : "No Position"
                             };
                return Ok(result);


            }

            catch (Exception ex)
            {
                //return BadRequest("ERROR");
                return BadRequest(ex.GetBaseException().ToString());
            }
        }

        public class EmployeeViewModel
        {

            public string? Id { get; set; }
            public string Department { get; set; }
            public string UserType { get; set; }
            public string EmployeeType { get; set; }
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
            public string Position { get; set; }
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
        private TblUsersModel buildEmployee(EmployeeViewModel registrationModel)
        {
            var pass = Cryptography.Encrypt(registrationModel.Password);
            var filepath = registrationModel.FilePath == null ? "" : registrationModel.FilePath;
            var BuffHerdModel = new TblUsersModel()
            {
                UserType = int.Parse(registrationModel.UserType),
                Fullname = registrationModel.Fname + " " + registrationModel.Mname + " " + registrationModel.Lname + " " + registrationModel.Suffix,
                Active = 1,
                Fname = registrationModel.Fname,
                Lname = registrationModel.Lname,
                Mname = registrationModel.Mname,
                Position = int.Parse(registrationModel.Position),
                Suffix = registrationModel.Suffix,
                Status = int.Parse(registrationModel?.Status),
                Department = int.Parse(registrationModel.Department),
                Email = registrationModel.Email,
                Gender = registrationModel.Gender,
                DateStarted = Convert.ToDateTime(registrationModel.DateStarted),
                DateCreated = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                CreatedBy = registrationModel.CreatedBy,
                PayrollType = int.Parse(registrationModel.PayrollType),
                SalaryType = int.Parse(registrationModel.SalaryType),
                Address = registrationModel.Address,
                FilePath = filepath,
                Username = registrationModel.Username,
                Password = pass,
                PositionLevelId = registrationModel.PositionLevelId,
                ManagerId = registrationModel.ManagerId,
                Rate = registrationModel.Rate,
                DaysInMonth = registrationModel.DaysInMonth,
                Cno = registrationModel.Cno,
                SSS_Number = registrationModel.SSS_Number,
                PagIbig_MID_Number = registrationModel.PagIbig_MID_Number,
                PhilHealth_Number = registrationModel.PhilHealth_Number,
                Tax_Identification_Number = registrationModel.Tax_Identification_Number,
            };

            return BuffHerdModel;
        }
        [HttpPost]
        public async Task<IActionResult> saveemployee(EmployeeViewModel data)
        {
            string status = "";
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUsersModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblUsersModels?.Any(userModel => userModel.Email == data.Email)).GetValueOrDefault();

            string filePath = @"C:\data\employeesave.json"; // Replace with your desired file path
            dbmet.insertlgos(filePath, JsonSerializer.Serialize(data));
            try
            {
                var EmployeeModel = buildEmployee(data);
                if (data.Id == null)
                {

                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }

                    _context.TblUsersModels.Add(EmployeeModel);
                    await _context.SaveChangesAsync();

                    status = "Employee successfully saved";
                    dbmet.InsertAuditTrail("Save Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", data.CreatedBy, "0");
                }
                else
                {
                    var employee = _context.TblUsersModels.SingleOrDefault(a => a.Id == int.Parse(data.Id));
                    employee.UserType = int.Parse(data.UserType);
                    employee.EmployeeType = int.Parse(data.EmployeeType);
                    employee.Fullname = data.Fname + " " + data.Mname + " " + data.Lname + " " + data.Suffix;
                    employee.Active = 1;
                    employee.Fname = data.Fname;
                    employee.Lname = data.Lname;
                    employee.Mname = data.Mname;
                    employee.Position = int.Parse(data.Position);
                    employee.Suffix = data.Suffix;
                    employee.Status = int.Parse(data?.Status);
                    employee.Department = int.Parse(data.Department);
                    employee.Email = data.Email;
                    employee.Gender = data.Gender;
                    employee.DateStarted = Convert.ToDateTime(data.DateStarted);
                    employee.DateCreated = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    employee.CreatedBy = data.CreatedBy;
                    employee.PayrollType = int.Parse(data.PayrollType);
                    employee.SalaryType = int.Parse(data.SalaryType);
                    employee.Address = data.Address;
                    if(data.FilePath != null)
                    {
                        employee.FilePath = data.FilePath;
                    }
                    employee.PositionLevelId = data.PositionLevelId;
                    employee.ManagerId = data.ManagerId;
                    employee.Rate = data.Rate;
                    employee.DaysInMonth = data.DaysInMonth;
                    employee.Cno = data.Cno;
                    employee.SSS_Number = data.SSS_Number;
                    employee.PagIbig_MID_Number = data.PagIbig_MID_Number;
                    employee.PhilHealth_Number = data.PhilHealth_Number;
                    employee.Tax_Identification_Number = data.Tax_Identification_Number;
                    _context.Entry(employee).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Employee" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", data.CreatedBy, "0");

                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPost]
        public async Task<IActionResult> updateEmployeeDetails(EmployeeViewModel data)
        {
            string status = "";
            try
            {
                var employee = _context.TblUsersModels.SingleOrDefault(a => a.Id == int.Parse(data.Id));
                employee.Fname = data.Fname;
                employee.Lname = data.Lname;
                employee.Mname = data.Mname;
                employee.Fullname = data.Fname + " " + data.Mname + " " + data.Lname + " " + data.Suffix;
                employee.Suffix = data.Suffix;
                employee.Gender = data.Gender;
                employee.Address = data.Address;
                employee.Cno = data.Cno;
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(status);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, TblUsersModel tblUserModel)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUsersModels'  is null.");
            }

            var userModel = _context.TblUsersModels.AsNoTracking().Where(userModel => !userModel.DeleteFlag && userModel.Id == id).FirstOrDefault();

            if (userModel == null)
            {
                return Conflict("No records matched!");
            }

            if (id != userModel.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblUsersModels?.Any(userModel => userModel.Username == tblUserModel.Username || userModel.Email == tblUserModel.Email)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(tblUserModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                string status = "Employee successfully updated";
                dbmet.InsertAuditTrail("Update Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", tblUserModel.CreatedBy, "0");

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Employee" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", tblUserModel.CreatedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel data)
        {

            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUsersModels'  is null.");
            }

            var userModel = await _context.TblUsersModels.FindAsync(data.id);
            if (userModel == null || userModel.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                userModel.DeleteFlag = true;
                userModel.DateDeleted = DateTime.Now;
                userModel.DeletedBy = data.deletedBy;
                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Employee successfully deleted";
                dbmet.InsertAuditTrail("Delete Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", data.deletedBy, "0");

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete Employee" + " " + ex, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", data.deletedBy, "0");

                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUsersModels'  is null.");
            }

            var userModel = await _context.TblUsersModels.FindAsync(restorationModel.id);
            if (userModel == null || !userModel.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                userModel.DeleteFlag = !userModel.DeleteFlag;
                userModel.DateDeleted = null;
                userModel.DeletedBy = "";
                userModel.DateRestored = DateTime.Now;
                userModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Employee successfully restored";
                dbmet.InsertAuditTrail("Restore Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", restorationModel.restoredBy, "0");

                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Restore Employee" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", restorationModel.restoredBy, "0");

                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TblUsersModel>> search(int id)
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUsersModels'  is null.");
            }
            var userModel = await _context.TblUsersModels.FindAsync(id);

            if (userModel == null || userModel.DeleteFlag)
            {
                return Conflict("No records found!");
            }

            string status = "Employee successfully searched";
            dbmet.InsertAuditTrail("Search Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", "User", "0");
            return Ok(userModel);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> view()
        {
            if (_context.TblUsersModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUsersModels'  is null.");
            }

            string status = "Employee successfully viewed";
            dbmet.InsertAuditTrail("View Active Employee" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", "User", "0");
            return await _context.TblUsersModels.Where(employeeModel => !employeeModel.DeleteFlag).ToListAsync();
        }

        // Email Modal
        public class UnregisteredUserEmailRequest
        {
            public string[] Name { get; set; }
            public string[] Email { get; set; }
            public string[] Username { get; set; }
            public string[] Password { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> EmailUnregisterUser(UnregisteredUserEmailRequest data)
        {
            var registrationDomain = "http://localhost:64539/";//domain
            var url = "https://eportal.odeccisolutions.com/";
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Odecci", "info@odecci.com"));
            for (int x = 0; x < data.Name.Length; x++)
            {
                //url = registrationDomain + "registration?empid=" + data.EmployeeId[x] + "&compid=" + data.CompanyId[x] + "&email=" + data.Email[x];
                message.To.Add(new MailboxAddress(data.Name[x], data.Email[x]));

                //var recipients = data.Name.Zip(data.Email, (name, email) => new MailboxAddress(name, email)).ToList();

                // Add all recipients at once
                //message.To.AddRange(recipients);

                message.Subject = "Email Registration Link";
                var bodyBuilder = new BodyBuilder();

                bodyBuilder.HtmlBody = @"<html>  
                                            <head>
                                                <style>
                                                    @font-face {font-family: 'Montserrat-Reg';src: url('/fonts/Montserrat/Montserrat-Regular.ttf');}
                                                    @font-face {
                                                    font-family: 'Montserrat-Bold';
                                                    src: url('/fonts/Montserrat/Montserrat-Bold.ttf');
                                                    }
                                                    @font-face {
                                                    font-family: 'Montserrat-SemiBold';
                                                    src: url('/fonts/Montserrat/Montserrat-SemiBold.ttf');
                                                    }
    
                                                    body {
                                                        margin: 0;
                                                        box-sizing: border-box;
                                                        font-family: ""Ubuntu"", Sans-serif;
                                                        color: #fff;
                                                    }
                                                    .container {
                                                        background-color: #102B3C;
                                                        padding: 25px;
                                                    }
                                                    .gradient-border {
                                                        background-color: transparent;
                                                        border: 3px solid #fff;
                                                        padding: 50px;
                                                    }
                                                    .container img {
                                                        height: 100px;
                                                    }
                                                    h1 {
                                                        font-size: 1rem;
                                                        color: #fff;
                                                    }
                                                    h3 {
                                                        font-size: 1.5rem;
                                                        color: #fff;
                
                                                    }
                                                    h4 {
                                                        font-size: .8rem;
                                                        text-decoration: none;
                                                        color: #fff;
                                                        padding: 0;
                                                        margin: 0;
                                                    }
                                                </style>
                                            </head>
                                            <body>
                                                <div class='container'>
                                                    <div class='gradient-border'>
                                                        <img src='http://ec2-54-251-135-135.ap-southeast-1.compute.amazonaws.com:8090/img/logo-04.png' alt='ODECCI' /> <br/><br/>
                                                        <h1 style='color: #fff'>WELCOME TO ODECCI " + data.Name[x].ToUpper() + "</h1>"
                                                        + "<h4 style='color: #fff'>To keep your account safe, we recommend you to change your password immediately.</h4> <br/><br/>"
                                                        + "<h1 style='color: #fff'>"
                                                           + "This is your Login Credentials: <br />"
                                                        + "</h1>"
                                                        + "<h4 style='color: #fff'>Username: " + data.Username[x] + "</h4>"
                                                        + "<h4 style='color: #fff'>Password:" + data.Password[x] + "</h4> <br/><br/>"
                                                    + "<a href='" + url + "'><h4> Click Here to Login </h4></a>"
                                                    + "</div>"
                                                + "</div> "
                                            + "</body>"
                                        + "</html>";
                message.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("info@odecci.com", "Roq30573");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                }
            }
            return Ok();
        }

        //EmergencyContact
        [HttpPost]
        public async Task<ActionResult<TblEmergencyContactsModel>> EmergencyContact(TblEmergencyContactsModel data)
        {

            if (data.UserId == null)
            {
                string sql = $@"SELECT TOP 1 ID FROM tbl_usersmodel ORDER BY ID DESC";
                // Fetch the result as a DataTable
                DataTable table = db.SelectDb(sql).Tables[0];
                // Check if there are any rows in the result
                if (table.Rows.Count > 0)
                {
                    // Parse and save the ID from the first row
                    data.UserId = int.Parse(table.Rows[0]["Id"].ToString());
                }
            }
            if (data.Id == 0)
            {
                _context.TblEmergencyContactsModel.Add(data);
            }
            else
            {
                _context.Entry(data).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        
        
        //Save Required Documents
        [HttpPost]
        public async Task<ActionResult> SaveRequiredDocuments(List<tbl_UsersRequiredDocuments> list)
        {
            try 
            { 
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].FilePath == "" || list[i].FilePath == null)
                    {
                        var file = _context.tbl_UsersRequiredDocuments.SingleOrDefault(a => a.Id == list[i].Id);
                        file.isDeleted = true;
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                
                    var userId = list[i].UserId;
                    if (userId == null || userId == 0)
                    {
                        string sql = $@"SELECT TOP 1 ID FROM tbl_usersmodel ORDER BY ID DESC";
                        // Fetch the result as a DataTable
                        DataTable table = db.SelectDb(sql).Tables[0];
                        // Check if there are any rows in the result
                        if (table.Rows.Count > 0)
                        {
                            // Parse and save the ID from the first row
                            list[i].UserId = int.Parse(table.Rows[0]["Id"].ToString());
                        }
                    }
                    if (list[i].Id == 0)
                    {
                        var item = new tbl_UsersRequiredDocuments();
                        item.UserId = list[i].UserId;
                        item.FileName = list[i].FileName;
                        item.FilePath = list[i].FilePath;
                        item.FileType = list[i].FileType;
                        item.isDeleted = false;
                        _context.tbl_UsersRequiredDocuments.Add(item);
                    }
                    else
                    {
                        _context.Entry(list).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
            return Ok();
        }
        public class RequiredDocumentsParam
        {
            public int UserId { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> PostRequiredDocuments(RequiredDocumentsParam data)
        {
            var result = _context.tbl_UsersRequiredDocuments.Where(a => a.UserId == data.UserId && a.isDeleted == false).ToList();
            
            return Ok(result);
        }
        public class PositionLevel
        {
            public int Id { get; set; }
            public string Level { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetPositionLevels()
        {
            string sql = $@"SELECT * FROM tbl_PositionLevel where DeleteFlag = 0";

            // Assuming `db.SelectDb(sql)` executes the query and returns a DataSet
            DataTable table = db.SelectDb(sql).Tables[0];

            // Check if the table contains any rows
            if (table.Rows.Count == 0)
            {
                return NotFound("No record found with the specified condition.");
            }
            var result = new List<PositionLevel>();
            foreach (DataRow dr in table.Rows)
            {
                var item = new PositionLevel();
                item.Id = int.Parse(dr["Id"].ToString());
                item.Level = dr["level"].ToString();
                result.Add(item);
            }

            return Ok(result);
        }
        public class Manager
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetManager()
        {
            string sql = $@"SELECT Id ,Fullname FROM tbl_UsersModel with(nolock) where Delete_Flag = 0 and PositionLevelId = '5' or UserType = '2'";

            // Assuming `db.SelectDb(sql)` executes the query and returns a DataSet
            DataTable table = db.SelectDb(sql).Tables[0];

            // Check if the table contains any rows
            if (table.Rows.Count == 0)
            {
                return NotFound("No record found with the specified condition.");
            }
            var result = new List<Manager>();
            foreach (DataRow dr in table.Rows)
            {
                var item = new Manager();
                item.Id = int.Parse(dr["Id"].ToString());
                item.Name = dr["Fullname"].ToString();
                result.Add(item);
            }

            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> saveemploymentstatus(TblEmploymentStatusModel data)
        {
            string status = "";
            if (_context.TblEmploymentStatusModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblEmploymentStatusModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblEmploymentStatusModels?.Any(schedule => schedule.Title == data.Title)).GetValueOrDefault();
            var existingSched = _context.TblEmploymentStatusModels?.Where(a => a.Id == data.Id).FirstOrDefault();

            try
            {
                if (data.Id == null || data.Id == 0)
                {

                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    data.DateCreated = DateTime.Now;
                    data.DateUpdated = null;
                    data.DateDeleted = null;
                    data.StatusID = 1;
                    _context.TblEmploymentStatusModels.Add(data);
                    await _context.SaveChangesAsync();
                    status = "Successfully Save!";
                }
                else
                {
                    data.DateCreated = existingSched?.DateCreated;
                    var EmploymentStatus = _context.TblEmploymentStatusModels.SingleOrDefault(a => a.Id == data.Id);
                    EmploymentStatus.Id = data.Id;
                    EmploymentStatus.Title = data.Title;
                    EmploymentStatus.ScheduleId = data.ScheduleId;
                    EmploymentStatus.Description = data.Description;
                    EmploymentStatus.DateUpdated = DateTime.Now;


                    await _context.SaveChangesAsync();
                    status = "Successfully Update!";

                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeTypeCount()
        {

            try
            {
                var result = (from empType in _context.TblEmployeeTypes
                              join user in _context.TblUsersModels
                              on empType.Id equals user.UserType into userGroup
                              from user in userGroup.DefaultIfEmpty() // Left Join
                              group user by empType.Title into grouped
                              select new
                              {
                                  Title = grouped.Key,
                                  UserCount = grouped.Count(x => x != null)
                              })
              .OrderByDescending(x => x.UserCount)
              .ToList();
                int totalUsers = result.Sum(x => x.UserCount);
                var finalResult = result.Append(new { Title = "Total", UserCount = totalUsers }).ToList();
                return Ok(finalResult);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        public class Params
        {
            public int? Usertype { get; set; }
            public DateTime? datefrom { get; set; }
            public DateTime? dateto { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetTotalRenderedHoursList(Params data)
        {

            try
            {
                string startDate = Convert.ToDateTime(data.datefrom).ToString("yyyy-MM-dd");// ✅ Fixed Invalid Date
                string endDate = Convert.ToDateTime(data.dateto).ToString("yyyy-MM-dd");   // ✅ End Date
                int userTypeFilter = 3; // Example UserType to filter
                var excludedTaskIds = new List<int> { 10, 11, 12 };
                List<UserHoursReport> result = _context.TblTimeLogs
                    .Where(t => t.Date.Value >= Convert.ToDateTime(startDate)
                        && t.Date.Value <= Convert.ToDateTime(endDate) && !excludedTaskIds.Contains(t.TaskId.Value)) // ✅ Filter by Date Range
                    .Join(_context.TblUsersModels.Where(u => u.UserType == data.Usertype), // ✅ Filter by UserType
                          t => t.UserId,
                          u => u.Id,
                          (t, u) => new { t, u })
                    .GroupBy(x => new { x.t.UserId, x.u.Username, x.u.Fullname })
                    .Select(g => new UserHoursReport
                    {
                        UserId = int.Parse(g.Key.UserId.ToString()),
                        Username = g.Key.Username,
                        Fullname = g.Key.Fullname,
                        ApprovedHours = g.Where(x => x.t.StatusId == 1).Sum(x => (decimal?)x.t.RenderedHours) ?? 0,
                        BreakHours = g.Where(x => x.t.StatusId == 5).Sum(x => (decimal?)x.t.RenderedHours) ?? 0,
                        PendingHours = g.Where(x => x.t.StatusId == 0).Sum(x => (decimal?)x.t.RenderedHours) ?? 0,
                        TotalRenderedHours = g.Sum(x => (decimal?)x.t.RenderedHours) ?? 0
                    })
                    .OrderByDescending(x => x.ApprovedHours)
                    .ToList();


                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
