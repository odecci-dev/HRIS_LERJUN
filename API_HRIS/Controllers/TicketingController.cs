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
    public class TicketingController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public TicketingController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> isLoggedIn(loginCredentials data)
        {

            string status = "";
            var result = (dynamic)null;
            data.password = Cryptography.Encrypt(data.password);
            bool loginstats = _context.TblUsersModels.Where(a => a.isLoggedIn == true && a.Username == data.username && a.Password == data.password).ToList().Count() > 0;
            if (loginstats == true)
            {
                result = _context.TblUsersModels.Where(a => a.isLoggedIn == true && a.Username == data.username && a.Password == data.password).ToList();
                status = "Logged In";
                return Ok(result);
            }
            else
            {
                status = "You're not logged in in HRIS";
                return Ok(status);
            }
        }
        public class UserId
        {
            public int Id { get; set; }
        }

    }
}
