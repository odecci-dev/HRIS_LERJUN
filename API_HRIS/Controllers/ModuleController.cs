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
    public class ModuleController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();

        public ModuleController(ODC_HRISContext context)
        {
            _context = context;
        }

        public class TimeLogsParam
        {
            public string? Usertype { get; set; }
            public string? UserId { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> ModuleList()
        {
            var result = _context.TblModulesModel.ToList();
            return Ok(result);
        }

    }
}
