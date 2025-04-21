using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeePortalController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public EmployeePortalController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }
        public partial class EmployeePortalLoginPortal
        {
            public string? username { get; set; }
            public string? password { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> Login(EmployeePortalLoginPortal data)
        {

            string status = "";
            //var result = (dynamic)null;
            try
            {
                data.password = Cryptography.Encrypt(data.password);
                //bool loginstats = _context.TblUsersModels.Where(a => a.Username == data.username && a.Password == data.password).ToList().Count() > 0;
                bool loginstats = _context.TblUsersModels
                    .Any(a => a.Username == data.username && a.Password == data.password);

                if (loginstats == true)
                {
                    //result = _context.TblUsersModels.Where(a => a.Username == data.username && a.Password == data.password).ToList();

                    var result = _context.TblUsersModels
                                    .Where(a => a.Username == data.username && a.Password == data.password)
                                    .OrderByDescending(a => a.DateCreated)
                                    .ThenByDescending(a => a.DateUpdated)
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
                                        a.EmployeeId,
                                        a.FilePath,
                                        a.Cno,
                                        a.Address,
                                        a.Department,
                                        a.UserType,
                                        a.EmployeeType,
                                        a.SalaryType,
                                        a.Rate,
                                        a.DaysInMonth,
                                        a.PayrollType,
                                        a.DateStarted,
                                        a.Position,
                                        a.PositionLevelId,
                                        a.ManagerId,
                                        a.DateCreated,
                                        a.DateUpdated
                                        // skip IsActive, RoleId, etc. if they're problematic
                                    })
                                    .ToList();
                    status = "Logged In";
                    return Ok(result);
                }
                else
                {
                    status = "Error: Wrong Username or Password!";
                    return Ok(status);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

    }
}
