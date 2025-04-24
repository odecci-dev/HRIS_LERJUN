using API_HRIS.Manager;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Globalization;
using System;
using Microsoft.IdentityModel.Tokens;
using PeterO.Numbers;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class DeductionController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public DeductionController(ODC_HRISContext context,DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }


        [HttpGet]
        public async Task<IActionResult> GetSSSList()
        {
            var result = _context.TblSSSModel.Where(a => a.isActive == true).ToList();
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPagibigList()
        {
            var result = _context.TblPagIbigModel.Where(a => a.isActive == true).ToList();
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPhilHealthList()
        {
            var result = _context.TblPhilHealthModel.Where(a => a.isActive == true).ToList();
            return Ok(result);
        }
        public class TaxModel
        {
            public decimal? Tax_From { get; set; }
            public decimal? Tax_To { get; set; }
            public decimal? PrescribeWithHoldingTax { get; set; }
            public string? TaxTypeName { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetTaxList()
        {
            var dbtax = _context.TblTaxModel.Where(a => a.isActive == true).ToList();
            var dbtaxtype = _context.TblTaxTypeModel.Where(a => a.isDeleted != true).ToList();

            //var result = _context.TblTaxModel.Where(a => a.isActive == true).ToList();
            var result = from tax in dbtax
                         join taxType in dbtaxtype
                         on tax.TaxTypeId equals taxType.Id into taxGroup
                         from taxType in taxGroup.DefaultIfEmpty()

                         group new
                         {
                             TaxTypeId = taxType.Id
                         }
                         by new
                         {
                             TaxId = tax.Id,
                             TaxFrom = tax.Tax_From,
                             TaxTo = tax.Tax_To,
                             PrescribeWithHoldingTax = tax.PrescribeWithHoldingTax,
                             TaxType = taxType.Name
                         } into tax
                         select new TaxModel
                         {
                             Tax_From = tax.Key.TaxFrom,
                             Tax_To = tax.Key.TaxTo,
                             PrescribeWithHoldingTax = tax.Key.PrescribeWithHoldingTax,
                             TaxTypeName = tax.Key.TaxType,
                         };
            return Ok(result);
        }

        public partial class TblPhilhealthImportModel
        {
            public string? Salary_From { get; set; }
            public string? Salary_To { get; set; }
            public string? Monthly_Premium { get; set; }
            public string? Employer_Share { get; set; }
            public string? Employee_Share { get; set; }
        }
        
        [HttpPost]
        public async Task<IActionResult> ImportPhilhealth(List<TblPhilhealthImportModel> list)
        {
            string result = "";
            string status = "";
            try
            {
                var items = _context.TblPhilHealthModel.ToList();

                foreach (var item in items)
                {
                    item.isActive = false;
                    item.isDeleted = true;
                    item.DateDeleted = DateTime.Now.Date;
                }
                _context.SaveChanges();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = new TblPhilHealthModel();
                    item.Id = 0;
                    item.Salary_From = decimal.Parse(list[i].Salary_From);
                    item.Salary_To = decimal.Parse(list[i].Salary_To);
                    item.Monthly_Premium = decimal.Parse(list[i].Monthly_Premium);
                    item.Employer_Share = decimal.Parse(list[i].Employer_Share);
                    item.Employee_Share = decimal.Parse(list[i].Employee_Share);
                    item.isDeleted = false;
                    item.Status = 1;
                    item.DateCreated = DateTime.Now.Date;
                    item.DateDeleted = null;
                    item.isActive = true;
                    _context.TblPhilHealthModel.Add(item);
                    await _context.SaveChangesAsync();
                    status = "Inserted Successfully";
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());

            }

            return Content(status);
        }

        public partial class TblTaxImportModel
        {
            public string? Tax_From { get; set; }
            public string? Tax_To { get; set; }
            public string? PrescribeWithHoldingTax { get; set; }
            public string? TaxTypeId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ImportTax(List<TblTaxImportModel> list)
        {
            string result = "";
            string status = "";
            try
            {
                var items = _context.TblTaxModel.ToList();

                foreach (var item in items)
                {
                    item.isActive = false;
                    item.isDeleted = true;
                    item.DateDeleted = DateTime.Now.Date;
                }
                _context.SaveChanges();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = new TblTaxModel();
                    item.Id = 0;
                    item.Tax_From = decimal.Parse(list[i].Tax_From);
                    item.Tax_To = decimal.Parse(list[i].Tax_To);
                    item.PrescribeWithHoldingTax = decimal.Parse(list[i].PrescribeWithHoldingTax);
                    item.TaxTypeId = int.Parse(list[i].TaxTypeId);
                    item.isDeleted = false;
                    item.Status = 1;
                    item.DateCreated = DateTime.Now.Date;
                    item.DateDeleted = null;
                    item.isActive = true;
                    _context.TblTaxModel.Add(item);
                    await _context.SaveChangesAsync();
                    status = "Inserted Successfully";
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());

            }

            return Content(status);
        }
        public partial class TblSSSImportModel
        {
            public string? Compensation_From { get; set; }
            public string? Compensation_To { get; set; }
            public string? Employer_RegularSS { get; set; }
            public string? Employer_MPF { get; set; }
            public string? Employer_EC { get; set; }
            public string? Employer_Total { get; set; }
            public string? Employee_RegularSS { get; set; }
            public string? Employee_MPF { get; set; }
            public string? Employee_Total { get; set; }
            public string? Total_Contribution { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ImportSSS(List<TblSSSImportModel> list)
        {
            string result = "";
            string status = "";
            try
            {
                var items = _context.TblSSSModel.ToList();

                foreach (var item in items)
                {
                    item.isActive = false;
                    item.isDeleted = true;
                    item.DateDeleted = DateTime.Now.Date;
                }
                _context.SaveChanges();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = new TblSSSModel();
                    item.Id = 0;

                    item.Compensation_From = decimal.Parse(list[i].Compensation_From);
                    item.Compensation_To = decimal.Parse(list[i].Compensation_To);
                    item.Employer_RegularSS = decimal.Parse(list[i].Employer_RegularSS);
                    item.Employer_MPF = decimal.Parse(list[i].Employer_MPF);
                    item.Employer_EC = decimal.Parse(list[i].Employer_EC);
                    item.Employer_Total = decimal.Parse(list[i].Employer_Total);
                    item.Employee_RegularSS = decimal.Parse(list[i].Employee_RegularSS);
                    item.Employee_MPF = decimal.Parse(list[i].Employee_MPF);
                    item.Employee_Total = decimal.Parse(list[i].Employee_Total);
                    item.Total_Contribution = decimal.Parse(list[i].Total_Contribution);

                    item.isDeleted = false;
                    item.Status = 1;
                    item.DateCreated = DateTime.Now.Date;
                    item.DateDeleted = null;
                    item.isActive = true;
                    _context.TblSSSModel.Add(item);
                    await _context.SaveChangesAsync();
                    status = "Inserted Successfully";
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());

            }

            return Content(status);
        }
        public partial class TblPagibigImportModel
        {
            public string? Tax_From { get; set; }
            public string? Tax_To { get; set; }
            public string? Employee_Contribution_Rate { get; set; }
            public string? Employer_Contribution_Rate { get; set; }
            public string? Total_Contribution { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ImportPagibig(List<TblPagibigImportModel> list)
        {
            string result = "";
            string status = "";
            try
            {
                var items = _context.TblPagIbigModel.ToList();

                foreach (var item in items)
                {
                    item.isActive = false;
                    item.isDeleted = true;
                    item.DateDeleted = DateTime.Now.Date;
                }
                _context.SaveChanges();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = new TblPagIbigModel();
                    item.Id = 0;
                    item.Tax_From = decimal.Parse(list[i].Tax_From);
                    item.Tax_To = decimal.Parse(list[i].Tax_To);
                    item.Employee_Contribution_Rate = decimal.Parse(list[i].Employee_Contribution_Rate);
                    item.Employer_Contribution_Rate = decimal.Parse(list[i].Employer_Contribution_Rate);
                    item.Total_Contribution = decimal.Parse(list[i].Total_Contribution);
                    item.isDeleted = false;
                    item.Status = 1;
                    item.DateCreated = DateTime.Now.Date;
                    item.DateDeleted = null;
                    item.isActive = true;
                    _context.TblPagIbigModel.Add(item);
                    await _context.SaveChangesAsync();
                    status = "Inserted Successfully";
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());

            }

            return Content(status);
        }
    }
}
