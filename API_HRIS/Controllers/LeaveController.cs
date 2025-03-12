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
    public class LeaveController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public LeaveController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        [HttpGet]
        public async Task<IActionResult> TaskList()
        {
            var result = _context.TblTaskModels.Where(a => a.Status == 1 && a.isBreak == 0).ToList();
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> BreakList()
        {
            var result = _context.TblTaskModels.Where(a => a.Status == 1 && a.isBreak == 1 || a.Id == 2).ToList();
            return Ok(result);
        }

    }
}
