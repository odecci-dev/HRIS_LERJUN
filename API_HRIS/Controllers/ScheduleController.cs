using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using HonkSharp.Fluency;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ScheduleController : Controller
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;
        public ScheduleController(ODC_HRISContext context , DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }



        [HttpPost]
        public async Task<IActionResult> saveschedule(TblScheduleModel data)
        {
            string status = "";
            if (_context.TblScheduleModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblScheduleModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblScheduleModels?.Any(schedule => schedule.Title == data.Title && schedule.StatusID == "1")).GetValueOrDefault();
            var existingSched = _context.TblScheduleModels?.Where(a => a.Id == data.Id).FirstOrDefault();
            if (data.Title == null)
            {

                string query = $@"UPDATE tbl_ScheduleModel
		                            SET StatusId = 0,
                                        DateDeleted =GETDATE() WHERE Id = '" + data.Id + "'";
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
                    data.StatusID = "1";
                    _context.TblScheduleModels.Add(data);
                    await _context.SaveChangesAsync();
                    status = "Successfully Save!";
                }
                else
                {
                    if (data.Title != "" || data.Title != null)
                    {
                        data.DateCreated = existingSched?.DateCreated;
                        var schedule = _context.TblScheduleModels.SingleOrDefault(a => a.Id == data.Id);
                        schedule.Id = data.Id;
                        schedule.Title = data.Title;
                        schedule.Description = data.Description;
                        schedule.MondayS = data.MondayS;
                        schedule.MondayE = data.MondayE;
                        schedule.TuesdayS = data.TuesdayS;
                        schedule.TuesdayE = data.TuesdayE;
                        schedule.WednesdayS = data.WednesdayS;
                        schedule.WednesdayE = data.WednesdayE;
                        schedule.ThursdayS = data.ThursdayS;
                        schedule.ThursdayE = data.ThursdayE;
                        schedule.FridayS = data.FridayS;
                        schedule.FridayE = data.FridayE;
                        schedule.SaturdayS = data.SaturdayS;
                        schedule.SaturdayE = data.SaturdayE;
                        schedule.SundayS = data.SundayS;
                        schedule.SundayE = data.SundayE;
                        schedule.DateUpdated = DateTime.Now;


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
        public async Task<IActionResult> ScheduleList()
        {
            var result = (dynamic)null;
            try
            {
                result = _context.TblScheduleModels.Where(a => a.StatusID == "1").ToList();
            }
            catch
            {
                return BadRequest("ERROR");
            }


            return Ok(result);
        }
    }
}
