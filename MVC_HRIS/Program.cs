
using Microsoft.AspNetCore.Authentication.Cookies;
using MVC_HRIS.Manager;
using MVC_HRIS.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(
    options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(10);  // Set session timeout
        //options.IdleTimeout = TimeSpan.FromSeconds(5);  // Set session timeout
        options.Cookie.HttpOnly = true;  // Make session cookie HttpOnly
        options.Cookie.IsEssential = true;  // Make session cookie essential
    }
);
// Register other services
builder.Services.AddTransient<QueryValueService>();
builder.Services.AddCors();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://ec2-54-251-135-135.ap-southeast-1.compute.amazonaws.com:8089", "http://ec2-54-251-135-135.ap-southeast-1.compute.amazonaws.com:8090/")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});


builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {

        options.ExpireTimeSpan = TimeSpan.FromDays(14); // How long the cookie is valid
        options.SlidingExpiration = true; // Extend expiration with activity
        options.LoginPath = "/Login/Index"; // your login path
        options.AccessDeniedPath = "/Login/Index";
    });
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Middleware to redirect to login if session is expired
app.Use(async (context, next) =>
{
    var path = context.Request.Path.ToString().ToLower();

    // Avoid infinite loop by excluding the login page and static files
    if (!path.StartsWith("/login") && !path.StartsWith("/css") && !path.StartsWith("/js") && !context.Session.Keys.Contains("UserName"))
    {
        context.Response.Redirect("/Login/Index");
        return;
    }

    await next();
});

app.UseCookiePolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LogIn}/{action=Index}/{id?}");

app.Run();
