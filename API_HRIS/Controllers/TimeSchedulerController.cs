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
    public class TimeSchedulerController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;


        public TimeSchedulerController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }
        [HttpPost]
        public async Task<IActionResult> Create(TblCalendarModel schedule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(schedule);
                    await _context.SaveChangesAsync();
                    return Ok("Successfully Created");
                }
                return BadRequest("Invalid");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

   

        // Edit Schedule (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(TblCalendarModel schedule)
        {
            try
            {
                var existingEntity = _context.TimeSchedules.Local.FirstOrDefault(e => e.Id == schedule.Id);
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

              
                if (ModelState.IsValid)
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();

                    return Ok("Successfully Created");
                }
                return BadRequest("Invalid");
            }
            catch (Exception ex)
            {
               return Problem(ex.GetBaseException().ToString());
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) 
        {
            if (id == 0) return BadRequest("Invalid ID");

            var schedule = await _context.TimeSchedules.FindAsync(id);
            if (schedule == null) return NotFound();

            _context.TimeSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return Ok("Successfully Deleted");
        }
        [HttpGet]
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                var schedules = from schedule in _context.TimeSchedules
                                join employment in _context.TblEmployeeTypes
                                on schedule.EmploymentTypeId equals employment.Id
                                select new
                                {
                                    schedule.Id,
                                    Title = employment.Title,
                                    schedule.isFixed,
                                    ShiftStartTime = schedule.ShiftStartTime,
                                    ShiftEndTime = schedule.ShiftEndTime,
                                    BreakStartTime = schedule.BreakStartTime,
                                    BreakEndTime = schedule.BreakEndTime,
                                    ScheduleDate = schedule.ScheduleDate
                                };

                var result = schedules.ToList();


                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }


        }

    }
}
