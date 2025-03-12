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
    public class SalaryTypeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public SalaryTypeController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        [HttpGet]
        public async Task<IActionResult> SalaryTypeList()
        {
            return Ok(_context.TblSalaryTypes.Where(a => a.DeleteFlag == 0).OrderByDescending(a => a.Id).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> SalaryTypePaginationList(FilterSalaryType data)
        {

            int pageSize = 25;
            //var model_result = (dynamic)null;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {

                var salaryType = _context.TblSalaryTypes.ToList();
                totalItems = salaryType.Count();
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                items = salaryType.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();

                var result = new List<SalaryTypePaginateModel>();
                var item = new SalaryTypePaginateModel();
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

                string status = "Salary type successfully viewed";
                dbmet.InsertAuditTrail("View All Salary type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", "User", "0");

                return Ok(result);


            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TblSalaryType>> save(TblSalaryType tblSalaryType)
        {
            if (_context.TblSalaryTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblSalaryTypes?.Any(a => a.SalaryType == tblSalaryType.SalaryType && a.DeleteFlag != 1)).GetValueOrDefault();
            if (tblSalaryType.SalaryType == null)
            {

                string query = $@"UPDATE tbl_SalaryType
		                            SET DeleteFlag = 1"
                            + " WHERE Id = '" + tblSalaryType.Id + "'";
                db.AUIDB_WithParam(query);

                string status = "Salary Type successfully Deleted";
                return Ok(status);
            }

            try
            {
                string status = "";
                //_context.TblPositionModels.Add(tblPositionModel);
                if (tblSalaryType.Id == 0)
                {

                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    _context.TblSalaryTypes.Add(tblSalaryType);

                    status = "Salary Type successfully saved";
                }
                else
                {
                    _context.Entry(tblSalaryType).State = EntityState.Modified;
                    status = "Salary Type successfully updated";
                }

                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Save Salary Type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");

                return CreatedAtAction("save", new { id = tblSalaryType.Id }, tblSalaryType);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Salary Type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, TblSalaryType tblSalaryType)
        {
            if (_context.TblSalaryTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblSalaryTypes'  is null.");
            }

            var salaryType = _context.TblSalaryTypes.AsNoTracking().Where(salaryType => salaryType.Id == id).FirstOrDefault();

            if (salaryType == null)
            {
                return Conflict("No records matched!");
            }

            if (id != salaryType.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblSalaryTypes?.Any(salaryType => salaryType.SalaryType == tblSalaryType.SalaryType)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(tblSalaryType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Salary type successfully updated";
                dbmet.InsertAuditTrail("Update Salary type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", tblSalaryType.CreatedBy, "0");

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Salary type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", tblSalaryType.CreatedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {

            if (_context.TblSalaryTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblSalaryTypes'  is null.");
            }

            var salaryType = await _context.TblSalaryTypes.FindAsync(deletionModel.id);
            if (salaryType == null || salaryType.DeleteFlag == 0)
            {
                return Conflict("No records matched!");
            }

            try
            {
                salaryType.DeleteFlag = 0;
                //userType.DateDeleted = DateTime.Now;
                //userType.DeletedBy = deletionModel.deletedBy;
                //userType.DateRestored = null;
                //userType.RestoredBy = "";
                _context.Entry(salaryType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Salary type successfully deleted";
                dbmet.InsertAuditTrail("Delete Salary type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", deletionModel.deletedBy, "0");

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete Salary type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", deletionModel.deletedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.TblSalaryTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblSalaryTypes'  is null.");
            }

            var salaryType = await _context.TblSalaryTypes.FindAsync(restorationModel.id);
            if (salaryType == null || salaryType.DeleteFlag == 1)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                salaryType.DeleteFlag = 1;
                //userType.DateDeleted = null;
                //userType.DeletedBy = "";
                //userType.DateRestored = DateTime.Now;
                //userType.RestoredBy = restorationModel.restoredBy;
                _context.Entry(salaryType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Salary type successfully restored";
                dbmet.InsertAuditTrail("Restore Salary type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", restorationModel.restoredBy, "0");

                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Restore Salary type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", restorationModel.restoredBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TblUserType>> search(int id)
        {
            if (_context.TblSalaryTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblSalaryTypes'  is null.");
            }
            var salaryType = await _context.TblSalaryTypes.FindAsync(id);

            if (salaryType == null)
            {
                return Conflict("No records found!");
            }

            string status = "Salary type successfully searched";
            dbmet.InsertAuditTrail("Search Salary type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", "User", "0");
            return Ok(salaryType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblSalaryType>>> view()
        {
            if (_context.TblSalaryTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblSalaryTypes'  is null.");
            }
            return await _context.TblSalaryTypes.Where(salaryType => salaryType.DeleteFlag == 1).ToListAsync();
            string status = "Salary typeSalary successfully viewed";
            dbmet.InsertAuditTrail("View Active E type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Salary type Module", "User", "0");
        }
    }
}
