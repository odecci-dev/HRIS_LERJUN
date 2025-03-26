
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
using static API_HRIS.ApplicationModel.EntityModels;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeeTypeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public EmployeeTypeController(ODC_HRISContext context,DBMethods _dbmet)
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
                        eType.ScheduleId = data.ScheduleId;
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
    

}
}
