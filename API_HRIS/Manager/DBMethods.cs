
using API_HRIS.ApplicationModel;
using API_HRIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using PeterO.Numbers;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static API_HRIS.ApplicationModel.EntityModels;
namespace API_HRIS.Manager
{
    public class DBMethods 
    {
        private readonly ODC_HRISContext _context;
        string sql = "";
        string Stats = "";
        string Mess = "";
        string JWT = "";
        DbManager db = new DbManager();
        // Inject DbContext through constructor
        public DBMethods(ODC_HRISContext context)
        {
            _context = context;
        }


        //DBMethods dbmet = new DBMethods();
        //private readonly PCC_DEVContext _context;

        #region Models
        public class AuditTrailModel
        {
            public int Id { get; set; }
            public string Actions { get; set; }
            public string Module { get; set; }
            public string DateCreated { get; set; }
            public string UserType { get; set; }
            public string Username { get; set; }

        }
        public string insertlgos(string filepath, string logs)
        {
            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data));


            //System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(data[0]));
            //



            // Read the existing content of the file
            string existingContent = System.IO.File.ReadAllText(filepath);

            // Create a StringBuilder to manipulate the content
            StringBuilder sb = new StringBuilder(existingContent);

            // Insert the new text at a specific position (e.g., at the beginning of the file)
            sb.Insert(0, logs + " \n------------------" + DateTime.Now + "--------------- \n");

            // Write the modified content back to the file
            System.IO.File.WriteAllText(filepath, sb.ToString());


            return "";


        }
        public partial class RegistrationModel
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string Fname { get; set; }

            public string? Lname { get; set; }

            public string? Mname { get; set; }

            public string Email { get; set; }

            public string Gender { get; set; }

            public string? EmployeeId { get; set; }

            public string Jwtoken { get; set; }

            public string? FilePath { get; set; }

            public int? Active { get; set; }

            public string? Cno { get; set; }

            public string? Address { get; set; }

            public int? Status { get; set; }
            public string? CreatedBy { get; set; }

            public int? CenterId { get; set; }

            public bool? AgreementStatus { get; set; }
            public string? UserType { get; set; }
        }

        #endregion

        public async Task<decimal> ComputeSSSContribution(decimal basicSalary)
        {
            decimal totalSSSContribution;
            // Fetch the correct SSS rate based on the employee's salary
            var sssRate = await _context.TblSSSModel
                .Where(sss => basicSalary >= sss.Compensation_From && basicSalary <= sss.Compensation_To)
                .FirstOrDefaultAsync();

            if (sssRate == null)
            {
                totalSSSContribution = 0;
                return Math.Round(totalSSSContribution, 2);
            }

            // Compute Employee SSS Deduction
            decimal employeeSSS = (sssRate.Employee_RegularSS ?? 0) + (sssRate.Employee_MPF ?? 0);

            // Compute Employer SSS Contribution
            decimal employerSSS = (sssRate.Employer_RegularSS ?? 0) + (sssRate.Employer_MPF ?? 0) + (sssRate.Employer_EC ?? 0);

            // Compute Total SSS Contribution
             totalSSSContribution = employeeSSS/* + employerSSS;*/;
            //decimal totalSSSContribution = employeeSSS + employerSSS;

            return Math.Round(totalSSSContribution, 2); // Employee's deduction only
        }
        public async Task<decimal> ComputePhilHealthDeduction(decimal salary)
        {
            // Fetch the correct PhilHealth deduction based on salary range
            var philHealthRate = await _context.TblPhilHealthModel
                .Where(p => salary >= p.Salary_From && salary <= p.Salary_To)
                .FirstOrDefaultAsync();

            if (philHealthRate == null)
            {
                throw new Exception("No matching PhilHealth rate found for this salary range.");
            }

            // Compute Employee PhilHealth Deduction
            decimal employeePhilHealth = philHealthRate.Employee_Share ?? 0;

            return Math.Round(employeePhilHealth, 2);  // Return employee's share for deduction
        }
        public async Task<decimal> ComputePagIbigDeduction(decimal salary)
        {
            // Fetch the correct Pag-IBIG contribution rate based on salary range
            var pagIbigRate = await _context.TblPagIbigModel
                .Where(p => salary >= p.Tax_From && salary <= p.Tax_To)
                .FirstOrDefaultAsync();

            if (pagIbigRate == null)
            {
                throw new Exception("No matching Pag-IBIG rate found for this salary range.");
            }

            // Compute Employee Pag-IBIG Contribution
            decimal employeePagIbig = (pagIbigRate.Employee_Contribution_Rate ?? 0) * salary;

            return Math.Round(employeePagIbig, 2);  // Return employee's share for deduction
        }
        public async Task<decimal> ComputeWithholdingTax(decimal salary, int? compensationType)
        {
            decimal tax = 0;
            var taxBracket = _context.TblTaxModel
                            .Where(t => t.TaxTypeId == compensationType && salary >= t.Tax_From && salary <= t.Tax_To)
                            .FirstOrDefault();
            if (taxBracket == null)
                return 0;
            tax = taxBracket.PrescribeWithHoldingTax ?? 0;
            decimal excessAmount = salary - (taxBracket.Tax_From ?? 0);

            decimal total_tax = excessAmount * taxBracket.PrescribeWithHoldingTax ?? 0;


            return Math.Round(total_tax, 2);
        }

        public string InsertAuditTrail(string actions, string datecreated, string module, string userid, string read)
        {
            string Insert = $@"INSERT INTO [dbo].[tbl_audittrail]
                           ([Actions]
                           ,[Module]
                           ,[DateCreated]
                           ,[UserId]
                           ,[status])
                         VALUES
                               ('" + actions + "'," +
                             "'" + module + "'," +
                             "'" + datecreated + "'," +
                             "'" + userid + "'," +
                              "'" + read + "') ";

            return db.DB_WithParam(Insert);
        }
        public List<TimelogsVM>  TimeLogsData()
        {
            var result = new List<TimelogsVM>();

            DataTable dt = db.SelectDb_SP("sp_timelogs").Tables[0];
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var item = new TimelogsVM();
                    item.Id = dr["Id"].ToString();
                    item.UserId = dr["UserId"].ToString();
                    item.Date = Convert.ToDateTime(dr["Date"].ToString()).ToString("yyyy-MM-dd");
                    item.TimeIn = dr["TimeIn"].ToString() == null ? "0:00": dr["TimeIn"].ToString();
                    item.TimeOut = dr["TimeOut"].ToString() == null ? "0:00" : dr["TimeOut"].ToString();
                    item.RenderedHours = dr["RenderedHours"].ToString() == null ? "0.00" : dr["RenderedHours"].ToString();
                    item.Username = dr["Username"].ToString();
                    item.Fname = dr["Fname"].ToString();
                    item.Lname = dr["Lname"].ToString();
                    item.Mname = dr["Mname"].ToString();
                    item.Suffix = dr["Suffix"].ToString();
                    item.Email = dr["Email"].ToString();
                    item.EmployeeID = dr["EmployeeID"].ToString();
                    item.JWToken = dr["JWToken"].ToString();
                    item.FilePath = dr["FilePath"].ToString();
                    item.UsertypeId = dr["UserTypeId"].ToString();
                    item.UserType = dr["UserType"].ToString();
                    item.DeleteFlagName = dr["DeleteFlagName"].ToString();
                    item.DeleteFlag = dr["DeleteFlag"].ToString();
                    item.StatusName = dr["StatusName"].ToString();
                    item.StatusId = dr["StatusId"].ToString();
                    item.Remarks = dr["Remarks"].ToString();
                    item.TaskId = dr["TaskId"].ToString();
                    item.Task = dr["Task"].ToString();
                    item.Department = dr["Department"].ToString();
                    result.Add(item);
                }
            }
            return result;
        }
        public StatusReturns GetUserLogIn(string username, string password, string? ipaddress, string? location)
        {
            var result = new List<TblUsersModel>();
            bool compr_user = false;
            string utype = "";
            if (username.Length != 0 || password.Length != 0)
            {
                var param = new IDataParameter[]
                {
                    new SqlParameter("@Username",username),
                    new SqlParameter("@Password",Cryptography.Encrypt(password))
                };
                DataTable dt = db.SelectDb_SP("GetUserLogIn", param).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    string user_statId = dt.Rows[0]["StatusId"].ToString();
                    string user_activeId = dt.Rows[0]["ActiveStatusId"].ToString();
                    utype = dt.Rows[0]["UserType"].ToString();
                    if (user_activeId == "1")
                    {
                        //active
                        switch (user_statId)
                        {
                            case "3":
                                //VERIFIED
                                Stats = "Error";
                                Mess = "Your account is under screening. Please contact administrator.";
                                JWT = "";
                                break;
                            case "4":
                                //UNVERIFIED
                                Stats = "Error";
                                Mess = "Your account is unverified. Please contact administrator.";
                                JWT = "";
                                break;

                            case "5":
                                Stats = "Error";
                                Mess = "Your account is not yet Activated. Please contact administrator.";
                                JWT = "";
                                break;
                            case "6":
                            //APPROVED
                            case "1003":
                            //ONBOARDING
                                compr_user = String.Equals(dt.Rows[0]["Username"].ToString().Trim(), username, StringComparison.Ordinal);

                                if (compr_user)
                                {
                                    string pass = Cryptography.Decrypt(dt.Rows[0]["password"].ToString().Trim());
                                    if ((pass).Equals(password))
                                    {
                                        StringBuilder str_build = new StringBuilder();
                                        Random random = new Random();
                                        int length = 8;
                                        char letter;

                                        for (int i = 0; i < length; i++)
                                        {
                                            double flt = random.NextDouble();
                                            int shift = Convert.ToInt32(Math.Floor(25 * flt));
                                            letter = Convert.ToChar(shift + 2);
                                            str_build.Append(letter);
                                        }
                                        //gv.AudittrailLogIn("Successfully", "Log In Form", dt.Rows[0]["EmployeeID"].ToString(), 7);
                                        var token = Cryptography.Encrypt(str_build.ToString());
                                        string strtokenresult = token;
                                        string[] charsToRemove = new string[] { "/", ",", ".", ";", "'", "=", "+" };
                                        foreach (var c in charsToRemove)
                                        {
                                            strtokenresult = strtokenresult.Replace(c, string.Empty);
                                        }

                                        string query = $@"update tbl_UsersModel set JWToken='" + string.Concat(strtokenresult.TakeLast(15)) + "' where id = '" + dt.Rows[0]["id"].ToString() + "'";
                                        db.DB_WithParam(query);

                                        Stats = "Ok";
                                        Mess = "Successfully Log In";
                                        JWT = string.Concat(strtokenresult.TakeLast(15));
                                    }
                                    else
                                    {
                                        string sql = $@"select * from tbl_Attempts where UserId ='" + dt.Rows[0]["Id"].ToString() + "'";
                                        DataTable user_dt = db.SelectDb(sql).Tables[0];
                                        if (user_dt.Rows.Count != 0)
                                        {
                                            //update
                                            int attemp_count = int.Parse(user_dt.Rows[0]["AttemptCount"].ToString());
                                            if (attemp_count > 5)
                                            {
                                                string update_attempts = $@"update tbl_Attempts set AttemptCount ='" + attemp_count + 1 + "'  where id ='" + dt.Rows[0]["Id"].ToString() + "'";
                                                db.DB_WithParam(update_attempts);
                                            }
                                            else
                                            {
                                                Stats = "Error";
                                                Mess = "User LogIn Attempts Exceeded. Please contact admin";
                                                JWT = "";
                                            }


                                        }
                                        else
                                        {
                                            string OTPInsert = $@"INSERT INTO [dbo].[tbl_Attempts]
                                                               ([UserId]
                                                               ,[AttemptCount]
                                                               ,[IPAddress]
                                                               ,[Location])
                                                                VALUES
                                                               ('" + dt.Rows[0]["Id"].ToString() + "'" +
                                                               ",'1'," +
                                                               "'" + ipaddress + "'," +
                                                               "'" + location + "')";
                                            db.DB_WithParam(OTPInsert);
                                            Stats = "Error";
                                            Mess = "Invalid Log In";
                                            JWT = "";
                                            //insert

                                        }
                                        //update login attempts

                                    }
                                }
                                break;
                            default:
                                break;
                        }


                    }
                    else
                    {
                        //inactive
                        Stats = "Error";
                        Mess = "User is InActive. Please contact your administrator. ";
                        JWT = "";

                    }

                }
                else
                {
                    Stats = "Error";
                    Mess = "Invalid LogIn";
                    JWT = "";
                }
                string sqls = $@"select Username from tbl_UsersModel where Username ='" + username + "'";
                DataTable table = db.SelectDb(sqls).Tables[0];
                InsertAuditTrail("Log In " + Stats + " " + Mess, DateTime.Now.ToString("yyyy-MM-dd"), "LogIn Module", username, "0");


            }
            else
            {


            }
            StatusReturns results = new StatusReturns
            {
                Status = Stats,
                Message = Mess,
                JwtToken = JWT,
                UserType = utype
            };
            return results;
        }

        public List<TblUsersModel_List> getUserList()
        {

            string sqls = $@"SELECT     
            tbl_UsersModel.Id, 
            tbl_UsersModel.Username, 
            tbl_UsersModel.Password, 
            tbl_UsersModel.Fullname, 
            tbl_UsersModel.Fname, 
            tbl_UsersModel.Lname, 
            tbl_UsersModel.Mname, 
            tbl_UsersModel.Email, 
            tbl_UsersModel.Gender, 
            tbl_UsersModel.EmployeeID, 
            tbl_UsersModel.JWToken, 
            tbl_UsersModel.FilePath, 
            tbl_UsersModel.Active, 
            tbl_UsersModel.Cno, 
            tbl_UsersModel.Address, 
            tbl_UsersModel.Status, 
            tbl_UsersModel.Date_Created, 
            tbl_UsersModel.Date_Updated, 
            tbl_UsersModel.Delete_Flag, 
            tbl_UsersModel.Created_By,
            tbl_UsersModel.Updated_By, 
            tbl_UsersModel.Date_Deleted, 
            tbl_UsersModel.Deleted_By, 
            tbl_UsersModel.Date_Restored, 
            tbl_UsersModel.Restored_By, 
            tbl_UsersModel.Department as DepartmentId, 
            tbl_UsersModel.AgreementStatus, 
            tbl_UsersModel.RememberToken, 
            tbl_StatusModel.Status AS StatusName, 
            tbl_DeparmentModel.DepartmentName
            FROM  
            tbl_UsersModel INNER JOIN
            tbl_StatusModel ON tbl_UsersModel.Status = tbl_StatusModel.id INNER JOIN
            tbl_DeparmentModel ON tbl_UsersModel.Department = tbl_DeparmentModel.id
            WHERE        (tbl_UsersModel.Delete_Flag = 0)";
            var result = new List<TblUsersModel_List>();
            DataTable table = db.SelectDb(sqls).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new TblUsersModel_List();
                item.Id = int.Parse(dr["Id"].ToString());
                item.Username = dr["Username"].ToString();
                item.Password = dr["Password"].ToString();
                item.Fullname = dr["Fullname"].ToString();
                item.Fname = dr["Fname"].ToString();
                item.Lname = dr["Lname"].ToString();
                item.Mname = dr["Mname"].ToString();
                item.Email = dr["Email"].ToString();
                item.Gender = dr["Gender"].ToString();
                item.EmployeeId = dr["EmployeeID"].ToString();
                item.Jwtoken = dr["JWToken"].ToString();
                item.FilePath = dr["FilePath"].ToString();
                item.Active = int.Parse(dr["Active"].ToString());
                item.Cno = dr["Cno"].ToString();
                item.Address = dr["Address"].ToString();
                item.Status = int.Parse(dr["Status"].ToString());
                item.DateCreated = dr["Date_Created"].ToString();
                item.DateUpdated = dr["Date_Updated"].ToString();
                item.DeleteFlag = Convert.ToBoolean(dr["Delete_Flag"].ToString());
                item.CreatedBy = dr["Created_By"].ToString();
                item.UpdatedBy = dr["Updated_By"].ToString();
                item.DateDeleted = dr["Date_Deleted"].ToString();
                item.CreatedBy = dr["Created_By"].ToString();
                item.UpdatedBy = dr["Updated_By"].ToString();
                item.DateDeleted = dr["Date_Deleted"].ToString();
                item.DeletedBy = dr["Deleted_By"].ToString();
                item.DateRestored = dr["Date_Restored"].ToString();
                item.RestoredBy = dr["Restored_By"].ToString();
                item.DepartmentId = int.Parse(dr["DepartmentId"].ToString());
                item.DepartmentName = dr["DepartmentName"].ToString();
                item.AgreementStatus = bool.Parse(dr["AgreementStatus"].ToString()) == null ? false : bool.Parse(dr["AgreementStatus"].ToString());
                item.RememberToken = dr["RememberToken"].ToString();
                //item.UserType = dr["UserType"].ToString();
                //item.UserTypeCode = dr["code"].ToString();
                //item.UserTypeName = dr["name"].ToString();
                item.StatusName = dr["StatusName"].ToString();

              
                result.Add(item);
            }
            return result;
        }

        public List<AuditTrailModel> GetAuditTrail()
        {

            string sql = $@"SELECT        tbl_audittrail.Id, tbl_audittrail.Actions, tbl_audittrail.Module, tbl_audittrail.DateCreated, tbl_audittrail.UserId, tbl_UserType.UserType AS UserType, tbl_UsersModel.Username
FROM            tbl_UserType INNER JOIN
                         tbl_UsersModel ON tbl_UserType.id = tbl_UsersModel.UserType INNER JOIN
                         tbl_audittrail ON tbl_UsersModel.Username = tbl_audittrail.UserId";
            var result = new List<AuditTrailModel>();
            DataTable table = db.SelectDb(sql).Tables[0];

            foreach (DataRow dr in table.Rows)
            {
                var item = new AuditTrailModel();
                item.Id = int.Parse(dr["Id"].ToString());
                item.Actions = dr["Actions"].ToString();
                item.Module = dr["Module"].ToString();
                item.DateCreated = dr["DateCreated"].ToString();
                item.Username = dr["Username"].ToString();
                item.UserType = dr["UserType"].ToString();
                result.Add(item);
            }
            return result;
        }
    }
}

