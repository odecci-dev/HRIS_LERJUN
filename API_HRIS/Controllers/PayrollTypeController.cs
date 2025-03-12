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
    public class PayrollTypeController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        private readonly DBMethods dbmet;
        DbManager db = new DbManager();

        public PayrollTypeController(ODC_HRISContext context, DBMethods dbmet)
        {
            _context = context;
            dbmet = dbmet;
        }
        [HttpGet]
        public async Task<IActionResult> PayrollTypeList()
        {
            return Ok(_context.TblPayrollTypes.Where(a => a.DeleteFlag == 0).OrderByDescending(a => a.Id).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> PayrollTypePaginationList(FilterPayrollType data)
        {

            int pageSize = 25;
            //var model_result = (dynamic)null;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;
            string page_size = pageSize == 0 ? "10" : pageSize.ToString();
            try
            {

                var payrollType = _context.TblPayrollTypes.ToList();
                totalItems = payrollType.Count();
                totalPages = (int)Math.Ceiling((double)totalItems / int.Parse(page_size.ToString()));

                items = payrollType.Skip((data.page - 1) * int.Parse(page_size.ToString())).Take(int.Parse(page_size.ToString())).ToList();

                var result = new List<PayrollTypePaginateModel>();
                var item = new PayrollTypePaginateModel();
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

                string status = "Payroll Type successfully viewed";
                dbmet.InsertAuditTrail("View All Payroll Type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");

                return Ok(result);


            }

            catch (Exception ex)
            {
                return BadRequest("ERROR");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TblPayrollType>> save(TblPayrollType tblPayrollType)
        {
            if (_context.TblPayrollTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblDeparmentModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblPayrollTypes?.Any(a => a.PayrollType == tblPayrollType.PayrollType && a.DeleteFlag != 1)).GetValueOrDefault();
            if (tblPayrollType.PayrollType == null)
            {

                string query = $@"UPDATE tbl_PayrollType
		                            SET DeleteFlag = 1"
                            + " WHERE Id = '" + tblPayrollType.Id + "'";
                db.AUIDB_WithParam(query);

                string status = "Payroll Type successfully Deleted";
                return Ok(status);
            }

            try
            {
                string status = "";
                //_context.TblPositionModels.Add(tblPositionModel);
                if (tblPayrollType.Id == 0)
                {

                    if (hasDuplicateOnSave)
                    {
                        return Conflict("Entity already exists");
                    }
                    _context.TblPayrollTypes.Add(tblPayrollType);

                    status = "Payroll Type successfully saved";
                }
                else
                {
                    _context.Entry(tblPayrollType).State = EntityState.Modified;
                    status = "Payroll Type successfully updated";
                }

                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Save Department" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");

                return CreatedAtAction("save", new { id = tblPayrollType.Id }, tblPayrollType);
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Save Department" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, TblPayrollType tblPayrollType)
        {
            if (_context.TblPayrollTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblPayrollTypes'  is null.");
            }

            var payrolltype = _context.TblPayrollTypes.AsNoTracking().Where(payrolltype => payrolltype.Id == id).FirstOrDefault();

            if (payrolltype == null)
            {
                return Conflict("No records matched!");
            }

            if (id != payrolltype.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblPayrollTypes?.Any(payrolltype => payrolltype.PayrollType == tblPayrollType.PayrollType)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(tblPayrollType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                string status = "Payroll type successfully updated";
                dbmet.InsertAuditTrail("Update Payroll Type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", tblPayrollType.CreatedBy, "0");

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                dbmet.InsertAuditTrail("Update Payroll Type" + " " + ex.Message, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", tblPayrollType.CreatedBy, "0");
                return Problem(ex.GetBaseException().ToString());
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> delete(DeletionModel deletionModel)
        //{

        //    if (_context.TblUserTypes == null)
        //    {
        //        return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
        //    }

        //    var userType = await _context.TblUserTypes.FindAsync(deletionModel.id);
        //    if (userType == null || userType.Status == 0)
        //    {
        //        return Conflict("No records matched!");
        //    }

        //    try
        //    {
        //        userType.Status = 0;
        //        userType.DateDeleted = DateTime.Now;
        //        userType.DeletedBy = deletionModel.deletedBy;
        //        userType.DateRestored = null;
        //        userType.RestoredBy = "";
        //        _context.Entry(userType).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        return Ok("Deletion Successful!");
        //    }
        //    catch (Exception ex)
        //    {

        //        return Problem(ex.GetBaseException().ToString());
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> restore(RestorationModel restorationModel)
        //{

        //    if (_context.TblUserTypes == null)
        //    {
        //        return Problem("Entity set 'ODC_HRISContext.TblUserTypes'  is null.");
        //    }

        //    var userType = await _context.TblUserTypes.FindAsync(restorationModel.id);
        //    if (userType == null || userType.Status == 1)
        //    {
        //        return Conflict("No deleted records matched!");
        //    }

        //    try
        //    {
        //        userType.Status = 1;
        //        userType.DateDeleted = null;
        //        userType.DeletedBy = "";
        //        userType.DateRestored = DateTime.Now;
        //        userType.RestoredBy = restorationModel.restoredBy;
        //        _context.Entry(userType).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        return Ok("Restoration Successful!");
        //    }
        //    catch (Exception ex)
        //    {

        //        return Problem(ex.GetBaseException().ToString());
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<TblUserType>> search(int id)
        {
            if (_context.TblPayrollTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblPayrollTypes'  is null.");
            }
            var payrollType = await _context.TblPayrollTypes.FindAsync(id);

            if (payrollType == null)
            {
                return Conflict("No records found!");
            }
            string status = "Payroll Type successfully searched";
            dbmet.InsertAuditTrail("Search Payroll Type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module Module", "User", "0");
            return Ok(payrollType);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblPayrollType>>> view()
        {
            if (_context.TblPayrollTypes == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblPayrollTypes'  is null.");
            }
            return await _context.TblPayrollTypes.ToListAsync();
            string status = "Payroll Type successfully viewed";
            dbmet.InsertAuditTrail("View Active Payroll Type" + " " + status, DateTime.Now.ToString("yyyy-MM-dd"), "Payroll Type Module", "User", "0");
        }
    }
}
