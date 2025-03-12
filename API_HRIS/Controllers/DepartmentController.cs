
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
    public class DepartmentController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        private readonly DBMethods dbmet;
        DbManager db = new DbManager();
        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public DepartmentController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentList()
        {
            return Ok(_context.TblDeparmentModels.Where(a => a.DeleteFlag == 0).OrderByDescending(a => a.Id).ToList());
        }
        [HttpPost]
        public async Task<IActionResult> DepartmentPaginationList(FilterDepartment data)
        {

            int pageSize = 25;
            //var model_result = (dynamic)null;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {

                var postion = _context.TblDeparmentModels.ToList();
                totalItems = postion.Count();
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                items = postion.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();

                var result = new List<DepartmentPaginationModel>();
                var item = new DepartmentPaginationModel();
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

                string status = "Department successfully viewed";
                dbmet.InsertAuditTrail("View All Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", "User", "0");

                return Ok(result);


            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TblDeparmentModel>> save(TblDeparmentModel tblDeparmentModel)
        {
            if (_context.TblDeparmentModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblDeparmentModels?.Any(departmentModel => departmentModel.DepartmentName == tblDeparmentModel.DepartmentName && departmentModel.DeleteFlag != 1)).GetValueOrDefault();
            if(tblDeparmentModel.DepartmentName == null)
            {

                string query = $@"UPDATE tbl_DeparmentModel
		                            SET DeleteFlag = 1"
                            + " WHERE Id = '" + tblDeparmentModel.Id + "'";
                db.AUIDB_WithParam(query);

                string status = "Department successfully Deleted";
                return Ok(status);
            }

            try
            {
                string status = "";
                //_context.TblPositionModels.Add(tblPositionModel);
                if (tblDeparmentModel.Id == 0)
                {

                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    _context.TblDeparmentModels.Add(tblDeparmentModel);


                    status = "Department successfully saved";
                }
                else
                {
                  
                    _context.Entry(tblDeparmentModel).State = EntityState.Modified;
                    status = "Department successfully updated";
                    
                    
                }

                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Save Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Departmente Module", "User", "0");

                return CreatedAtAction("save", new { id = tblDeparmentModel.Id }, tblDeparmentModel);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Department" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, TblDeparmentModel tblDepartmentModel)
        {
            if (_context.TblDeparmentModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }

            var departmentModel = _context.TblDeparmentModels.AsNoTracking().Where(departmentModel =>  departmentModel.Id == id).FirstOrDefault();

            if (departmentModel == null)
            {
                return Conflict("No records matched!");
            }

            if (id != departmentModel.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblDeparmentModels?.Any(departmentModel => departmentModel.DepartmentName == tblDepartmentModel.DepartmentName)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(tblDepartmentModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Department successfully updated";
                dbmet.InsertAuditTrail("Update Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", tblDepartmentModel.CreatedBy, "0");

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Department" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", tblDepartmentModel.CreatedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {

            if (_context.TblDeparmentModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }

            var departmentModel = await _context.TblDeparmentModels.FindAsync(deletionModel.id);
            if (departmentModel == null || departmentModel.DeleteFlag == 0)
            {
                return Conflict("No records matched!");
            }

            try
            {
                departmentModel.DeleteFlag = 0;
                _context.Entry(departmentModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Department successfully deleted";
                dbmet.InsertAuditTrail("Delete Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", deletionModel.deletedBy, "0");

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Delete Department" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", deletionModel.deletedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
            }

            [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.TblDeparmentModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }

            var departmentModel = await _context.TblDeparmentModels.FindAsync(restorationModel.id);
            if (departmentModel == null || departmentModel.DeleteFlag == 1)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                departmentModel.DeleteFlag = 1;
                _context.Entry(departmentModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Department successfully restored";
                dbmet.InsertAuditTrail("Restore Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", restorationModel.restoredBy, "0");

                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Restore Department" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", restorationModel.restoredBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TblDeparmentModel>> search(int id)
        {
            if (_context.TblDeparmentModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }
            var departmentModel = await _context.TblDeparmentModels.FindAsync(id);

            if (departmentModel == null || departmentModel.DeleteFlag == 0)
            {
                return Conflict("No records found!");
            }

            string status = "Department successfully searched";
            dbmet.InsertAuditTrail("Search Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", "User", "0");
            return Ok(departmentModel);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblDeparmentModel>>> view()
        {
            if (_context.TblDeparmentModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }
            return await _context.TblDeparmentModels.Where(deparmentModel => deparmentModel.DeleteFlag == 1).ToListAsync();
            string status = "Department successfully viewed";
            dbmet.InsertAuditTrail("View Active Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Department Module", "User", "0");
        }
    }
}
