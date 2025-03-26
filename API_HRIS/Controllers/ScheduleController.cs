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
using System.Linq;

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

        public class TblScheduleModelRequest
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public bool? DeleteFlag { get; set; }
            public DateTime? DateCreated { get; set; }
            public DateTime? DateUpdated { get; set; }
            public DateTime? DateDeleted { get; set; }
            public string? MondayS { get; set; }
            public string? MondayE { get; set; }
            public string? TuesdayS { get; set; }
            public string? TuesdayE { get; set; }
            public string? WednesdayS { get; set; }
            public string? WednesdayE { get; set; }
            public string? ThursdayS { get; set; }
            public string? ThursdayE { get; set; }
            public string? FridayS { get; set; }
            public string? FridayE { get; set; }
            public string? SaturdayS { get; set; }
            public string? SaturdayE { get; set; }
            public string? SundayS { get; set; }
            public string? SundayE { get; set; }

            //public List<TblScheduleDayModel>? Schedule { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> saveschedule(TblScheduleModelRequest data)
        {
            string status = "";
            if (_context.TblScheduleModels == null)
            {
                return Problem("Entity set 'ODC_HRISContext.TblScheduleModels'  is null.");
            }
            bool hasDuplicateOnSave = (_context.TblScheduleModels?.Any(schedule => schedule.Title == data.Title && schedule.DeleteFlag == false)).GetValueOrDefault();
            var existingSched = _context.TblScheduleModels?.Where(a => a.Id == data.Id).FirstOrDefault();
            if (data.Title == null)
            {

                string query = $@"UPDATE tbl_ScheduleModel
		                            SET DeleteFlag = 1,
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
                    data.DeleteFlag = false;
                    var Schedule = new TblScheduleModel
                    {
                        Title = data.Title,
                        Description = data.Description,
                        DeleteFlag = false, // Default Active
                        DateCreated = DateTime.Now

                    };
                    _context.TblScheduleModels.Add(Schedule);
                    await _context.SaveChangesAsync();
                    var monday = new TblScheduleDayModel
                    {
                        StartTime = data.MondayS,
                        EndTime = data.MondayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 0
                    };
                    _context.TblScheduleDayModels.Add(monday);
                    await _context.SaveChangesAsync();
                    var tuesday = new TblScheduleDayModel
                    {
                        StartTime = data.TuesdayS,
                        EndTime = data.TuesdayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 1
                    };
                    _context.TblScheduleDayModels.Add(tuesday);
                    var wednesday = new TblScheduleDayModel
                    {
                        StartTime = data.WednesdayS,
                        EndTime = data.WednesdayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 2
                    };
                    _context.TblScheduleDayModels.Add(wednesday);
                    await _context.SaveChangesAsync();
                    var thursday = new TblScheduleDayModel
                    {
                        StartTime = data.ThursdayS,
                        EndTime = data.ThursdayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 3
                    };
                    _context.TblScheduleDayModels.Add(thursday);
                    await _context.SaveChangesAsync();
                    var friday = new TblScheduleDayModel
                    {
                        StartTime = data.FridayS,
                        EndTime = data.FridayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 4
                    };
                    _context.TblScheduleDayModels.Add(friday);
                    await _context.SaveChangesAsync();
                    var saturday = new TblScheduleDayModel
                    {
                        StartTime = data.SaturdayS,
                        EndTime = data.SaturdayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 5
                    };
                    _context.TblScheduleDayModels.Add(saturday);
                    await _context.SaveChangesAsync();
                    var sunday = new TblScheduleDayModel
                    {
                        StartTime = data.SundayS,
                        EndTime = data.SundayE,
                        SchceduleId = Schedule.Id, // Foreign Key (After saving team)
                        Day = 6
                    };
                    _context.TblScheduleDayModels.Add(sunday);
                    await _context.SaveChangesAsync();
                    status = "Successfully Save!";
                }
                else
                {
                   
                    var existingSchedule = await _context.TblScheduleModels.FindAsync(data.Id);
                    if (existingSchedule != null)
                    {

                        existingSchedule.Title = data.Title;
                        existingSchedule.Description = data.Title;
                        existingSchedule.DateUpdated = DateTime.Now;
                        _context.TblScheduleModels.Update(existingSchedule);
                        await _context.SaveChangesAsync();
                        status = "Successfully Update!";

                        // Remove existing team members before adding new ones (if necessary)
                        var existingSchedules = _context.TblScheduleDayModels.Where(m => m.SchceduleId == data.Id);
                        _context.TblScheduleDayModels.RemoveRange(existingSchedules);
                        await _context.SaveChangesAsync();

                        var monday = new TblScheduleDayModel
                        {
                            StartTime = data.MondayS,
                            EndTime = data.MondayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 0
                        };
                        _context.TblScheduleDayModels.Add(monday);
                        await _context.SaveChangesAsync();
                        var tuesday = new TblScheduleDayModel
                        {
                            StartTime = data.TuesdayS,
                            EndTime = data.TuesdayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 1
                        };
                        _context.TblScheduleDayModels.Add(tuesday);
                        var wednesday = new TblScheduleDayModel
                        {
                            StartTime = data.WednesdayS,
                            EndTime = data.WednesdayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 2
                        };
                        _context.TblScheduleDayModels.Add(wednesday);
                        await _context.SaveChangesAsync();
                        var thursday = new TblScheduleDayModel
                        {
                            StartTime = data.ThursdayS,
                            EndTime = data.ThursdayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 3
                        };
                        _context.TblScheduleDayModels.Add(thursday);
                        await _context.SaveChangesAsync();
                        var friday = new TblScheduleDayModel
                        {
                            StartTime = data.FridayS,
                            EndTime = data.FridayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 4
                        };
                        _context.TblScheduleDayModels.Add(friday);
                        await _context.SaveChangesAsync();
                        var saturday = new TblScheduleDayModel
                        {
                            StartTime = data.SaturdayS,
                            EndTime = data.SaturdayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 5
                        };
                        _context.TblScheduleDayModels.Add(saturday);
                        await _context.SaveChangesAsync();
                        var sunday = new TblScheduleDayModel
                        {
                            StartTime = data.SundayS,
                            EndTime = data.SundayE,
                            SchceduleId = data.Id, // Foreign Key (After saving team)
                            Day = 6
                        };
                        _context.TblScheduleDayModels.Add(sunday);
                        await _context.SaveChangesAsync();
                        status = "Successfully Save!";
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
            
            try
            {
                var schedule = _context.TblScheduleModels.Where(a => a.DeleteFlag ==false).ToList();
                var scheduleday = _context.TblScheduleDayModels.ToList();
                var result = from sched in schedule
                         
                         where sched.DeleteFlag == false // Filtering condition
                         
                         select new
                         {
                             Id = sched.Id,
                             Title = sched.Title,
                             Description = sched.Description,
                         };
                var finalResult = result.ToList();
                return Ok(finalResult);
            }
            catch
            {
                return BadRequest("ERROR");
            }
        }
        public class scheduleId {
            public int Id { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ScheduleById(scheduleId data)
        {

            try
            {
               
                var result = _context.TblScheduleDayModels.Where(a => a.SchceduleId == data.Id).OrderBy(a => a.Day).ToList();
                
                return Ok(result);
            }
            catch
            {
                return BadRequest("ERROR");
            }
        }
    }
}
