
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Cryptography;
using MimeKit;
using System.Security.Policy;
using MailKit.Net.Smtp;
using System;

namespace API_HRIS.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ODC_HRISContext _context;
        DbManager db = new DbManager();
        private readonly DBMethods dbmet;

        public class BirthTypesSearchFilter
        {
            public string? BirthTypeCode { get; set; }
            public string? BirthTypeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public UserController(ODC_HRISContext context, DBMethods _dbmet)
        {
            _context = context;
            dbmet = _dbmet;
        }

        // POST: BirthTypes/list

        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> login(loginCredentials data)
        {


            var item = new StatusReturns();
            //if (!data.rememberToken.IsNullOrEmpty())
            //{

            //    userModel = dbmet.getUserList().Where(userModel => userModel.Username == data.username).FirstOrDefault();
            //    //usertype = int.Parse(userModel.UserType);
            //    //userModel.RememberToken = data.rememberToken;
            //    //_context.Entry(userModel).State = EntityState.Modified;

            //    //await _context.SaveChangesAsync();
            //    string tbl_UsersModel_update = $@"UPDATE [dbo].[tbl_UsersModel] SET 
            //                                 [FirstName] = '" + data.rememberToken + "'" +
            //                            " WHERE id = '" + userModel.Id + "'";

            //    string result = db.DB_WithParam(tbl_UsersModel_update);
            //}

            var isLoggedin = _context.TblUsersModels.Where(a => a.Username == data.username && a.Password == Cryptography.Encrypt(data.password) && a.isLoggedIn == true).ToList().Count() > 0;
            if (isLoggedin)
            {
                item.Status = "Error! Your account is active on another device or browser!";
                item.Message = "Invalid Log In";
                item.JwtToken = "";
                item.UserType = "";
            }
            else
            {
                var loginstats = dbmet.GetUserLogIn(data.username, data.password, data.ipaddress, data.location);
                item.Status = loginstats.Status;
                item.Message = loginstats.Message;
                item.JwtToken = loginstats.JwtToken;
                item.UserType = loginstats.UserType;
            }

            return Ok(item);
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> isLoggedIn(loginCredentials data)
        {

            string status = "";
            var result = (dynamic)null;
            data.password = Cryptography.Encrypt(data.password);
            bool loginstats = _context.TblUsersModels.Where(a => a.isOnline == true && a.Username == data.username && a.Password == data.password).ToList().Count() > 0;
            if (loginstats == true)
            {
                result = _context.TblUsersModels.Where(a => a.isOnline == true && a.Username == data.username && a.Password == data.password).ToList();
                status = "Logged In";
                return Ok(result);
            }
            else
            {
                status = "You're not logged in in HRIS";
                return Ok(status);
            }
        }
        public class ForgotPasswordParam
        {
            public string email { get; set; }
            public string? vcode { get; set; }
            public string? newPassword { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> SearchAccount(ForgotPasswordParam data)
        {

            string status = "";
            var result = (dynamic)null;
            bool existing = _context.TblUsersModels.Where(a => a.Email == data.email).ToList().Count() > 0;
            if (existing == true)
            {
                bool isActive = _context.TblUsersModels.Where(a => a.Email == data.email && a.Active == 1 && a.DeleteFlag == false).ToList().Count() > 0;
                if (isActive == true)
                {
                    var existingItem = _context.TblUsersModels.Where(a => a.Email == data.email).ToList().FirstOrDefault();
                    if (existingItem != null)
                    {
                        const string chars = "0123456789";
                        var code = new char[6];

                        using (var rng = RandomNumberGenerator.Create())
                        {
                            byte[] randomBytes = new byte[6];

                            rng.GetBytes(randomBytes);

                            for (int i = 0; i < 6; i++)
                            {
                                code[i] = chars[randomBytes[i] % chars.Length];
                            }
                        }
                        var vcode = new string(code);
                        var message = new MimeMessage();

                        message.From.Add(new MailboxAddress("Odecci", "info@odecci.com"));

                        //url = registrationDomain + "registration?empid=" + data.EmployeeId[x] + "&compid=" + data.CompanyId[x] + "&email=" + data.Email[x];
                        message.To.Add(new MailboxAddress(existingItem.Fullname, existingItem.Email));

                        //var recipients = data.Name.Zip(data.Email, (name, email) => new MailboxAddress(name, email)).ToList();

                        // Add all recipients at once
                        //message.To.AddRange(recipients);

                        message.Subject = "Reset Password Verification Code";
                        var bodyBuilder = new BodyBuilder();

                        bodyBuilder.HtmlBody = @"<html>  
                                        <head>
                                            <style>
                                                @font-face {font-family: 'Montserrat-Reg';src: url('/fonts/Montserrat/Montserrat-Regular.ttf');}
                                                @font-face {
                                                font-family: 'Montserrat-Bold';
                                                src: url('/fonts/Montserrat/Montserrat-Bold.ttf');
                                                }
                                                @font-face {
                                                font-family: 'Montserrat-SemiBold';
                                                src: url('/fonts/Montserrat/Montserrat-SemiBold.ttf');
                                                }
    
                                                body {
                                                    margin: 0;
                                                    box-sizing: border-box;
                                                    font-family: ""Ubuntu"", Sans-serif;
                                                    color: #102B3C;
                                                }
                                                .container {
                                                    background-color: #fff;
                                                    padding: 25px;
                                                    text-align: center;
                                                }
                                                .gradient-border {
                                                    background-color: transparent;
                                                    border: 3px solid #102B3C;
                                                    padding: 50px;
                                                }
                                                .container img {
                                                    height: 100px;
                                                }
                                                h1 {
                                                    font-size: 1rem;
                                                    color: #102B3C;
                                                }
                                                h3 {
                                                    font-size: 1.5rem;
                                                    color: #102B3C;
                
                                                }
                                                h4 {
                                                    font-size: .8rem;
                                                    text-decoration: none;
                                                    color: #102B3C;
                                                    padding: 0;
                                                    margin: 0;
                                                }
                                            </style>
                                        </head>
                                        <body>
                                            <div class='container'>
                                                <div class='gradient-border'>
                                                    
                                                    <svg width='250' viewBox='0 0 522 223' fill='none' xmlns='http://www.w3.org/2000/svg'>
                                                        <path d='M163.851 190.225C163.851 189.683 163.767 189.196 163.6 188.765C163.447 188.32 163.155 187.916 162.724 187.555C162.293 187.179 161.688 186.81 160.909 186.449C160.13 186.087 159.121 185.712 157.883 185.322C156.506 184.877 155.199 184.376 153.961 183.82C152.737 183.264 151.652 182.617 150.706 181.88C149.774 181.128 149.037 180.259 148.495 179.271C147.966 178.284 147.702 177.136 147.702 175.829C147.702 174.563 147.98 173.416 148.536 172.386C149.093 171.343 149.872 170.453 150.873 169.716C151.875 168.965 153.057 168.387 154.42 167.984C155.797 167.581 157.306 167.379 158.947 167.379C161.187 167.379 163.141 167.782 164.81 168.589C166.479 169.396 167.773 170.502 168.691 171.906C169.623 173.311 170.089 174.918 170.089 176.726H163.871C163.871 175.836 163.684 175.057 163.308 174.389C162.946 173.708 162.39 173.172 161.639 172.783C160.902 172.393 159.97 172.199 158.843 172.199C157.758 172.199 156.854 172.365 156.131 172.699C155.408 173.019 154.865 173.457 154.503 174.014C154.142 174.556 153.961 175.168 153.961 175.85C153.961 176.364 154.086 176.83 154.337 177.248C154.601 177.665 154.99 178.054 155.505 178.416C156.02 178.778 156.652 179.118 157.404 179.438C158.155 179.758 159.024 180.071 160.012 180.377C161.667 180.878 163.12 181.441 164.372 182.067C165.638 182.693 166.695 183.396 167.544 184.175C168.392 184.954 169.032 185.837 169.463 186.824C169.894 187.812 170.11 188.932 170.11 190.183C170.11 191.505 169.852 192.687 169.338 193.73C168.823 194.774 168.079 195.657 167.105 196.38C166.132 197.103 164.97 197.653 163.621 198.028C162.272 198.404 160.763 198.592 159.094 198.592C157.591 198.592 156.11 198.397 154.649 198.008C153.189 197.604 151.861 196.999 150.664 196.192C149.482 195.386 148.536 194.356 147.827 193.104C147.117 191.853 146.763 190.371 146.763 188.66H153.043C153.043 189.606 153.189 190.406 153.481 191.06C153.773 191.713 154.183 192.242 154.712 192.645C155.255 193.049 155.894 193.341 156.632 193.522C157.383 193.703 158.203 193.793 159.094 193.793C160.178 193.793 161.069 193.64 161.764 193.334C162.474 193.028 162.995 192.604 163.329 192.061C163.677 191.519 163.851 190.907 163.851 190.225ZM173.031 187.116V186.678C173.031 185.023 173.267 183.5 173.74 182.109C174.213 180.704 174.902 179.487 175.806 178.458C176.71 177.428 177.823 176.629 179.144 176.058C180.465 175.474 181.982 175.182 183.692 175.182C185.403 175.182 186.926 175.474 188.262 176.058C189.597 176.629 190.717 177.428 191.621 178.458C192.539 179.487 193.234 180.704 193.707 182.109C194.18 183.5 194.417 185.023 194.417 186.678V187.116C194.417 188.758 194.18 190.281 193.707 191.686C193.234 193.077 192.539 194.294 191.621 195.337C190.717 196.366 189.604 197.166 188.283 197.736C186.961 198.307 185.445 198.592 183.734 198.592C182.023 198.592 180.5 198.307 179.165 197.736C177.843 197.166 176.724 196.366 175.806 195.337C174.902 194.294 174.213 193.077 173.74 191.686C173.267 190.281 173.031 188.758 173.031 187.116ZM179.04 186.678V187.116C179.04 188.062 179.123 188.946 179.29 189.766C179.457 190.587 179.721 191.31 180.083 191.936C180.458 192.548 180.945 193.028 181.543 193.376C182.141 193.723 182.872 193.897 183.734 193.897C184.569 193.897 185.285 193.723 185.883 193.376C186.481 193.028 186.961 192.548 187.323 191.936C187.684 191.31 187.949 190.587 188.116 189.766C188.296 188.946 188.387 188.062 188.387 187.116V186.678C188.387 185.76 188.296 184.898 188.116 184.091C187.949 183.27 187.677 182.547 187.302 181.921C186.94 181.281 186.46 180.781 185.862 180.419C185.264 180.057 184.541 179.877 183.692 179.877C182.844 179.877 182.121 180.057 181.523 180.419C180.938 180.781 180.458 181.281 180.083 181.921C179.721 182.547 179.457 183.27 179.29 184.091C179.123 184.898 179.04 185.76 179.04 186.678ZM204.452 166.127V198.174H198.423V166.127H204.452ZM222.771 192.771V175.599H228.78V198.174H223.126L222.771 192.771ZM223.439 188.139L225.212 188.097C225.212 189.599 225.038 190.997 224.69 192.291C224.343 193.57 223.821 194.683 223.126 195.629C222.43 196.561 221.554 197.291 220.497 197.82C219.44 198.334 218.195 198.592 216.762 198.592C215.663 198.592 214.648 198.439 213.716 198.133C212.798 197.813 212.005 197.319 211.337 196.651C210.684 195.97 210.169 195.1 209.793 194.043C209.432 192.972 209.251 191.686 209.251 190.183V175.599H215.26V190.225C215.26 190.893 215.336 191.456 215.489 191.915C215.656 192.374 215.886 192.75 216.178 193.042C216.47 193.334 216.811 193.543 217.2 193.668C217.604 193.793 218.049 193.856 218.536 193.856C219.774 193.856 220.747 193.605 221.457 193.104C222.18 192.604 222.688 191.922 222.98 191.06C223.286 190.183 223.439 189.21 223.439 188.139ZM244.428 175.599V179.856H231.284V175.599H244.428ZM234.538 170.029H240.547V191.373C240.547 192.026 240.631 192.527 240.798 192.875C240.978 193.223 241.243 193.466 241.59 193.605C241.938 193.73 242.376 193.793 242.905 193.793C243.28 193.793 243.614 193.779 243.906 193.751C244.212 193.71 244.47 193.668 244.678 193.626L244.699 198.049C244.185 198.216 243.628 198.348 243.03 198.446C242.432 198.543 241.771 198.592 241.048 198.592C239.727 198.592 238.572 198.376 237.585 197.945C236.611 197.5 235.86 196.79 235.331 195.817C234.803 194.843 234.538 193.563 234.538 191.978V170.029ZM254.192 175.599V198.174H248.163V175.599H254.192ZM247.787 169.716C247.787 168.839 248.093 168.116 248.705 167.546C249.317 166.976 250.138 166.69 251.167 166.69C252.183 166.69 252.996 166.976 253.608 167.546C254.234 168.116 254.547 168.839 254.547 169.716C254.547 170.592 254.234 171.315 253.608 171.886C252.996 172.456 252.183 172.741 251.167 172.741C250.138 172.741 249.317 172.456 248.705 171.886C248.093 171.315 247.787 170.592 247.787 169.716ZM258.198 187.116V186.678C258.198 185.023 258.435 183.5 258.908 182.109C259.381 180.704 260.069 179.487 260.973 178.458C261.877 177.428 262.99 176.629 264.312 176.058C265.633 175.474 267.149 175.182 268.86 175.182C270.571 175.182 272.094 175.474 273.429 176.058C274.765 176.629 275.884 177.428 276.788 178.458C277.706 179.487 278.402 180.704 278.875 182.109C279.348 183.5 279.584 185.023 279.584 186.678V187.116C279.584 188.758 279.348 190.281 278.875 191.686C278.402 193.077 277.706 194.294 276.788 195.337C275.884 196.366 274.771 197.166 273.45 197.736C272.129 198.307 270.613 198.592 268.902 198.592C267.191 198.592 265.668 198.307 264.332 197.736C263.011 197.166 261.891 196.366 260.973 195.337C260.069 194.294 259.381 193.077 258.908 191.686C258.435 190.281 258.198 188.758 258.198 187.116ZM264.207 186.678V187.116C264.207 188.062 264.291 188.946 264.458 189.766C264.625 190.587 264.889 191.31 265.25 191.936C265.626 192.548 266.113 193.028 266.711 193.376C267.309 193.723 268.039 193.897 268.902 193.897C269.736 193.897 270.453 193.723 271.051 193.376C271.649 193.028 272.129 192.548 272.49 191.936C272.852 191.31 273.116 190.587 273.283 189.766C273.464 188.946 273.554 188.062 273.554 187.116V186.678C273.554 185.76 273.464 184.898 273.283 184.091C273.116 183.27 272.845 182.547 272.469 181.921C272.108 181.281 271.628 180.781 271.03 180.419C270.432 180.057 269.708 179.877 268.86 179.877C268.011 179.877 267.288 180.057 266.69 180.419C266.106 180.781 265.626 181.281 265.25 181.921C264.889 182.547 264.625 183.27 264.458 184.091C264.291 184.898 264.207 185.76 264.207 186.678ZM289.161 180.419V198.174H283.152V175.599H288.785L289.161 180.419ZM288.284 186.094H286.657C286.657 184.425 286.873 182.923 287.304 181.587C287.735 180.238 288.34 179.091 289.119 178.145C289.898 177.185 290.823 176.455 291.894 175.954C292.979 175.439 294.189 175.182 295.524 175.182C296.581 175.182 297.548 175.335 298.424 175.641C299.301 175.947 300.052 176.434 300.678 177.102C301.318 177.769 301.804 178.653 302.138 179.751C302.486 180.85 302.66 182.192 302.66 183.778V198.174H296.609V183.757C296.609 182.756 296.47 181.977 296.192 181.421C295.914 180.864 295.503 180.475 294.961 180.252C294.432 180.016 293.779 179.897 293 179.897C292.193 179.897 291.491 180.057 290.893 180.377C290.308 180.697 289.821 181.142 289.432 181.713C289.056 182.269 288.771 182.923 288.577 183.674C288.382 184.425 288.284 185.232 288.284 186.094ZM319.435 191.936C319.435 191.505 319.31 191.115 319.059 190.768C318.809 190.42 318.343 190.1 317.661 189.808C316.994 189.502 316.027 189.224 314.761 188.973C313.621 188.723 312.556 188.41 311.569 188.034C310.595 187.645 309.747 187.179 309.023 186.637C308.314 186.094 307.758 185.454 307.354 184.717C306.951 183.966 306.749 183.111 306.749 182.151C306.749 181.205 306.951 180.315 307.354 179.48C307.772 178.646 308.363 177.908 309.128 177.269C309.907 176.615 310.853 176.107 311.965 175.745C313.092 175.37 314.358 175.182 315.763 175.182C317.724 175.182 319.407 175.495 320.812 176.121C322.23 176.747 323.315 177.609 324.067 178.708C324.832 179.793 325.214 181.031 325.214 182.422H319.205C319.205 181.838 319.08 181.316 318.83 180.857C318.593 180.384 318.218 180.016 317.703 179.751C317.202 179.473 316.548 179.334 315.742 179.334C315.074 179.334 314.497 179.452 314.01 179.689C313.523 179.911 313.148 180.217 312.883 180.607C312.633 180.982 312.508 181.4 312.508 181.859C312.508 182.206 312.577 182.519 312.716 182.798C312.869 183.062 313.113 183.305 313.447 183.528C313.781 183.75 314.212 183.959 314.74 184.154C315.283 184.335 315.95 184.501 316.743 184.654C318.371 184.988 319.824 185.426 321.104 185.969C322.383 186.497 323.399 187.221 324.15 188.139C324.901 189.043 325.277 190.232 325.277 191.707C325.277 192.708 325.054 193.626 324.609 194.461C324.164 195.295 323.524 196.025 322.69 196.651C321.855 197.263 320.853 197.743 319.685 198.091C318.531 198.425 317.23 198.592 315.783 198.592C313.683 198.592 311.903 198.216 310.442 197.465C308.996 196.714 307.897 195.761 307.146 194.607C306.408 193.438 306.04 192.242 306.04 191.018H311.736C311.764 191.839 311.972 192.499 312.362 193C312.765 193.501 313.273 193.863 313.885 194.085C314.511 194.308 315.185 194.419 315.909 194.419C316.688 194.419 317.334 194.315 317.849 194.106C318.364 193.883 318.753 193.591 319.017 193.23C319.296 192.854 319.435 192.423 319.435 191.936ZM346.85 167.796V198.174H340.612V167.796H346.85ZM358.221 180.419V198.174H352.212V175.599H357.846L358.221 180.419ZM357.345 186.094H355.717C355.717 184.425 355.933 182.923 356.364 181.587C356.795 180.238 357.401 179.091 358.179 178.145C358.958 177.185 359.883 176.455 360.954 175.954C362.039 175.439 363.249 175.182 364.585 175.182C365.642 175.182 366.609 175.335 367.485 175.641C368.361 175.947 369.112 176.434 369.738 177.102C370.378 177.769 370.865 178.653 371.199 179.751C371.546 180.85 371.72 182.192 371.72 183.778V198.174H365.67V183.757C365.67 182.756 365.531 181.977 365.252 181.421C364.974 180.864 364.564 180.475 364.021 180.252C363.493 180.016 362.839 179.897 362.06 179.897C361.253 179.897 360.551 180.057 359.953 180.377C359.369 180.697 358.882 181.142 358.492 181.713C358.117 182.269 357.832 182.923 357.637 183.674C357.442 184.425 357.345 185.232 357.345 186.094ZM385.616 193.897C386.353 193.897 387.007 193.758 387.577 193.48C388.147 193.188 388.592 192.785 388.912 192.27C389.246 191.741 389.42 191.122 389.434 190.413H395.088C395.074 191.999 394.65 193.41 393.815 194.648C392.981 195.872 391.861 196.839 390.456 197.549C389.051 198.244 387.48 198.592 385.741 198.592C383.988 198.592 382.458 198.3 381.151 197.715C379.857 197.131 378.779 196.324 377.917 195.295C377.055 194.252 376.408 193.042 375.977 191.665C375.545 190.274 375.33 188.786 375.33 187.2V186.595C375.33 184.995 375.545 183.507 375.977 182.13C376.408 180.739 377.055 179.529 377.917 178.5C378.779 177.456 379.857 176.643 381.151 176.058C382.444 175.474 383.961 175.182 385.699 175.182C387.549 175.182 389.17 175.537 390.561 176.246C391.966 176.956 393.064 177.971 393.857 179.292C394.664 180.6 395.074 182.151 395.088 183.945H389.434C389.42 183.194 389.26 182.512 388.954 181.9C388.662 181.288 388.231 180.802 387.661 180.44C387.104 180.064 386.416 179.877 385.595 179.877C384.719 179.877 384.002 180.064 383.446 180.44C382.89 180.802 382.458 181.302 382.152 181.942C381.846 182.568 381.631 183.284 381.506 184.091C381.394 184.884 381.339 185.719 381.339 186.595V187.2C381.339 188.076 381.394 188.918 381.506 189.724C381.617 190.531 381.826 191.248 382.132 191.873C382.451 192.499 382.89 192.993 383.446 193.355C384.002 193.716 384.726 193.897 385.616 193.897ZM398.865 195.274C398.865 194.384 399.171 193.64 399.783 193.042C400.409 192.444 401.236 192.145 402.265 192.145C403.295 192.145 404.115 192.444 404.727 193.042C405.353 193.64 405.666 194.384 405.666 195.274C405.666 196.165 405.353 196.909 404.727 197.507C404.115 198.105 403.295 198.404 402.265 198.404C401.236 198.404 400.409 198.105 399.783 197.507C399.171 196.909 398.865 196.165 398.865 195.274Z' fill='#0F2B3C'></path>
                                                        <path fill-rule='evenodd' clip-rule='evenodd' d='M54.5801 0C30.3718 9.92465 13.0028 28.5476 6.96158 41.8803C3.12832 49.3165 0.978027 57.6675 0.978027 66.4922C0.978027 97.6131 27.7202 122.842 60.7083 122.842C93.6964 122.842 120.439 97.6131 120.439 66.4922C120.439 35.3714 93.6964 10.1429 60.7083 10.1429C55.3405 10.1429 50.1381 10.8109 45.1894 12.0636L54.5801 0ZM60.7085 95.7939C77.5137 95.7939 91.1371 82.6751 91.1371 66.4922C91.1371 50.3094 77.5137 37.1906 60.7085 37.1906C43.9032 37.1906 30.2798 50.3094 30.2798 66.4922C30.2798 82.6751 43.9032 95.7939 60.7085 95.7939Z' fill='#0F2B3C'></path>
                                                        <path d='M176.05 72.1271V42.8255C153.771 38.9377 133.179 52.1006 127.592 71.8388C119.392 102.52 145.673 126.822 176.05 122.842C201.216 118.672 210.871 101.006 212.116 81.143V14.6508H181.124V81.143C180.473 87.1315 179.262 90.5228 176.05 92.4129C152.435 101.304 148.676 67.2445 176.05 72.1271Z' fill='#0F2B3C'></path>
                                                        <path d='M267.466 73.5099H248.307C248.655 71.0753 248.974 69.7512 250.569 67.8789C255.077 63.371 265.373 62.3755 270.292 73.5099C276.49 88.1647 263.889 106.194 250.006 93.7996L232.538 113.522C242.68 120.284 250.031 123.975 267.466 122.538C283.815 119.72 300.728 106.722 302.411 87.0337C304.665 70.1329 294.281 46.691 267.466 42.5178C248.685 38.532 215.43 48.4179 217.878 87.0337H262.958C269.254 83.4543 266.793 81.8541 262.958 79.7083C259.52 76.796 260.888 76.1165 267.466 73.5099Z' fill='#0F2B3C'></path>
                                                        <path fill-rule='evenodd' clip-rule='evenodd' d='M367.263 73.8175H392.964C388.685 55.436 371.598 41.6985 351.17 41.6985C327.519 41.6985 308.345 60.1153 308.345 82.8335C308.345 105.552 327.519 123.968 351.17 123.968C372.629 123.968 390.401 108.809 393.513 89.0318H367.545C364.686 95.0201 358.429 99.1746 351.171 99.1746C341.212 99.1746 333.139 91.3538 333.139 81.7063C333.139 72.0589 341.212 64.238 351.171 64.238C358.199 64.238 364.288 68.1336 367.263 73.8175Z' fill='#0F2B3C'></path>
                                                        <ellipse cx='351.171' cy='81.7065' rx='14.6508' ry='14.0873' fill='#ED1C24'></ellipse>
                                                        <path fill-rule='evenodd' clip-rule='evenodd' d='M441.973 123.968C463.432 123.968 481.204 108.809 484.316 89.0318H458.348C455.488 95.0201 449.231 99.1746 441.973 99.1746C432.014 99.1746 423.941 91.3538 423.941 81.7063C423.941 72.0589 432.014 64.238 441.973 64.238C449.002 64.238 455.091 68.1336 458.066 73.8175H483.767C479.488 55.436 462.401 41.6985 441.973 41.6985C418.321 41.6985 399.148 60.1153 399.148 82.8335C399.148 105.552 418.321 123.968 441.973 123.968ZM484.799 82.8335L484.799 82.7633V82.9037L484.799 82.8335Z' fill='#0F2B3C'></path>
                                                        <path fill-rule='evenodd' clip-rule='evenodd' d='M456.042 85.651C454.268 91.5113 448.64 95.7938 441.973 95.7938C433.882 95.7938 427.322 89.4867 427.322 81.7065C427.322 73.9263 433.882 67.6191 441.973 67.6191C449.061 67.6191 454.973 72.4589 456.331 78.889H483.672V85.651H456.042Z' fill='#ED1C24'></path>
                                                        <rect x='489.95' y='43.9527' width='31.5556' height='78.8891' fill='#0F2B3C'></rect>
                                                        <ellipse cx='505.165' cy='25.9207' rx='15.2143' ry='14.6508' fill='#0F2B3C'></ellipse>
                                                    </svg>
                                                    <h1 style='color: #102B3C'>WELCOME TO ODECCI</h1>"
                                                                + "<h4 style='color: #102B3C'>We received a request to reset the password for your HIRS account associated with this email address.</h4> <br/><br/>"
                                                                + "<h1 style='color: #102B3C'>"
                                                                    + "To proceed, please use the verification code below: <br />"
                                                                + "</h1>"
                                                                + "<h4 style='color: #102B3C'>Verification Code: " + vcode + "</h4>"
                                                            + "</div>"
                                                        + "</div> "
                                                    + "</body>"
                                                + "</html>";
                        message.Body = bodyBuilder.ToMessageBody();
                        using (var client = new SmtpClient())
                        {
                            await client.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                            await client.AuthenticateAsync("info@odecci.com", "Roq30573");
                            await client.SendAsync(message);
                            await client.DisconnectAsync(true);

                        }
                        existingItem.verificationCode = vcode;
                        _context.TblUsersModels.Update(existingItem);
                        await _context.SaveChangesAsync();
                        dbmet.InsertAuditTrail("Verification code has been sent!", DateTime.Now.ToString("yyyy-MM-dd"), "Forgot Password Module", "User", data.email.ToString());
                    }

                    status = "Ok";
                    return Ok(status);
                }
                else
                {
                    status = "Your Account is not Active, please contact your Manager";
                    return Ok(status);
                }
            }
            else
            {
                status = "Your Email Address is not registered in HRIS, please contact your Manager";
                return Ok(status);
            }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> SearchAccountByVerificationCode(ForgotPasswordParam data)
        {

            string status = "";
            var result = (dynamic)null;
            bool existing = _context.TblUsersModels.Where(a => a.Email == data.email && a.Active == 1 && a.DeleteFlag == false && a.verificationCode == data.vcode).ToList().Count() > 0;
            if (existing == true)
            {
                status = "Ok";
                return Ok(status);
            }
            else
            {
                status = "Oops! That code doesn’t seem right. Please double-check and try again.";
                return Ok(status);
            }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> SaveNewPassword(ForgotPasswordParam data)
        {

            string status = "";
            var result = (dynamic)null;
            var pass = Cryptography.Encrypt(data.newPassword);
            bool existing = _context.TblUsersModels.Where(a => a.Email == data.email && a.Active == 1 && a.DeleteFlag == false && a.verificationCode == data.vcode).ToList().Count() > 0;
            if (existing == true)
            {
                var existingItem = _context.TblUsersModels.Where(a => a.Email == data.email && a.Active == 1 && a.DeleteFlag == false && a.verificationCode == data.vcode).ToList().FirstOrDefault();

                existingItem.Password = pass; _context.TblUsersModels.Update(existingItem);
                await _context.SaveChangesAsync();
                dbmet.InsertAuditTrail("Password has been change!", DateTime.Now.ToString("yyyy-MM-dd"), "Forgot Password Module", "User", data.email.ToString());

                status = "Ok";
                return Ok(status);
            }
            else
            {
                status = "Oops! That password doesn’t seem right. Please double-check and try again.";
                return Ok(status);
            }
        }

        public class UserIdParam
        {
            public int? Id { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> LogOut(UserIdParam data)
        {

            string status = "";
            var result = (dynamic)null;
            bool existing = _context.TblUsersModels.Where(a => a.Id == data.Id).ToList().Count() > 0;
            if (existing == true)
            {
                string queryIsLoggedIn = $@"UPDATE [dbo].[tbl_UsersModel] SET [isLoggedIn] = '0'" +
                                       " WHERE id = '" + data.Id + "'";
                db.DB_WithParam(queryIsLoggedIn);

                dbmet.InsertAuditTrail("Logout!", DateTime.Now.ToString("yyyy-MM-dd"), "Logout Module", "User", data.Id.ToString());

                status = "Ok";
                return Ok(status);
            }
            else
            {
                status = "Oops! Error";
                return Ok(status);
            }
        }


    }
}
