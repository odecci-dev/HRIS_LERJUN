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
    public class UserTypeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;


        public UserTypeController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }
        [HttpGet]
        public async Task<IActionResult> UserTypeList()
        {
            return Ok(_context.TblUserTypes.Where(a => a.Status == 1).ToList());
        }
        [HttpPost]
        public async Task<IActionResult> UserTypePaginationList(FilterUserType data)
        {

            int pageSize = 25;
            //var model_result = (dynamic)null;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {

                var userrType = _context.TblUserTypes.ToList();
                totalItems = userrType.Count();
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                items = userrType.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();

                var result = new List<UserTypePaginateModel>();
                var item = new UserTypePaginateModel();
                int pages = data.page == 0 ? 1 : data.page;
                item.CurrentPage = data.page == 0 ? "1" : data.page.ToString();

                int page_prev = pages - 1;

                double t_records = Math.Ceiling(double.Parse(totalItems.ToString()) / double.Parse(page_size));
                int page_next = data.page >= t_records ? 0 : pages + 1;
                item.NextPage = items.Count % int.Parse(page_size) >= 0 ? page_next.ToString() : "0";
                item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
                item.TotalPage = t_records.ToString();
                item.PageSize = page_size;
                item.TotalRecord = totalItems.ToString();
                item.items = items;
                result.Add(item);

                string status = "User type successfully viewed";
                dbmet.InsertAuditTrail("View All User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", "User", "0");

                return Ok(result);


            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TblUserType>> save(TblUserType tblUserType)
        {
            if (_context.TblUserTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblUserTypes?.Any(userType => userType.UserType == tblUserType.UserType)).GetValueOrDefault();


            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {

                tblUserType.Status = 1;



                _context.TblUserTypes.Add(tblUserType);
                await _context.SaveChangesAsync();

                string status = "User type successfully saved";
                dbmet.InsertAuditTrail("Save User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", tblUserType.CreatedBy, "0");

                return CreatedAtAction("save", new { id = tblUserType.Id }, tblUserType);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save User type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Employee Module", tblUserType.CreatedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, TblUserType tblUserType)
        {
            if (_context.TblUserTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
            }

            var departmentModel = _context.TblUserTypes.AsNoTracking().Where(departmentModel => departmentModel.Id == id).FirstOrDefault();

            if (departmentModel == null)
            {
                return Conflict("No records matched!");
            }

            if (id != departmentModel.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblUserTypes?.Any(usertype => usertype.UserType == tblUserType.UserType)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(tblUserType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "User type successfully updated";
                dbmet.InsertAuditTrail("Update User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", tblUserType.CreatedBy, "0");

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update User type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", tblUserType.CreatedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {

            if (_context.TblUserTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
            }

            var userType = await _context.TblUserTypes.FindAsync(deletionModel.id);
            if (userType == null || userType.Status == 0)
            {
                return Conflict("No records matched!");
            }

            try
            {
                userType.Status = 0;
                userType.DateDeleted = DateTime.Now;
                userType.DeletedBy = deletionModel.deletedBy;
                userType.DateRestored = null;
                userType.RestoredBy = "";
                _context.Entry(userType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "User type successfully deleted";
                dbmet.InsertAuditTrail("Delete User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", deletionModel.deletedBy, "0");

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete User type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", deletionModel.deletedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.TblUserTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
            }

            var userType = await _context.TblUserTypes.FindAsync(restorationModel.id);
            if (userType == null || userType.Status == 1)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                userType.Status = 1;
                userType.DateDeleted = null;
                userType.DeletedBy = "";
                userType.DateRestored = DateTime.Now;
                userType.RestoredBy = restorationModel.restoredBy;
                _context.Entry(userType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "User type successfully restored";
                dbmet.InsertAuditTrail("Restore User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", restorationModel.restoredBy, "0");

                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Restore User type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", restorationModel.restoredBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TblUserType>> search(int id)
        {
            if (_context.TblUserTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
            }
            var userType = await _context.TblUserTypes.FindAsync(id);

            if (userType == null || userType.Status == 0)
            {
                return Conflict("No records found!");
            }

            string status = "User type successfully searched";
            dbmet.InsertAuditTrail("Search User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", "User", "0");

            return Ok(userType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblUserType>>> view()
        {
            if (_context.TblUserTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
            }
            return await _context.TblUserTypes.Where(userType => userType.Status == 1).ToListAsync();
            string status = "User type successfully viewed";
            dbmet.InsertAuditTrail("View Active User type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "User type Module", "User", "0");
        }
    }
}
