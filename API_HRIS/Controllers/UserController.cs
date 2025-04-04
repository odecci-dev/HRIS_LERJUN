
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public UserController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        // POST: BirthTypes/list

        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> login(loginCredentials data)
        {
            var userModel = (dynamic)null;
            int usertype = 0;
            var loginstats = dbmet.GetUserLogIn(data.username, data.password, data.ipaddress, data.location);
           
            if (!data.rememberToken.IsNullOrEmpty())
            {

                userModel = dbmet.getUserList().Where(userModel => userModel.Username == data.username).FirstOrDefault();
                //usertype = int.Parse(userModel.UserType);
                //userModel.RememberToken = data.rememberToken;
                //_context.Entry(userModel).State = EntityState.Modified;

                //await _context.SaveChangesAsync();
                string tbl_UsersModel_update = $@"UPDATE [dbo].[tbl_UsersModel] SET 
                                             [FirstName] = '" + data.rememberToken + "'" +
                                        " WHERE id = '" + userModel.Id + "'";
               
                string result = db.DB_WithParam(tbl_UsersModel_update);
            }
            
            
            var item = new StatusReturns();
            item.Status = loginstats.Status;
            item.Message = loginstats.Message;
            item.JwtToken = loginstats.JwtToken;
            item.UserType = loginstats.UserType;

            return Ok(item);
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
            else {
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
