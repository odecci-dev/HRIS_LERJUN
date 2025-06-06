using API_HRIS.Models;
using API_HRIS;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using API_HRIS.Manager;
IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Path.GetPathRoot(Environment.SystemDirectory))
        .AddJsonFile("app/hris/appconfig.json", optional: true, reloadOnChange: true)
        .Build();
////STAGING
//IConfiguration config = new ConfigurationBuilder()
//  .SetBasePath(Path.GetPathRoot(Environment.SystemDirectory))
//  .AddJsonFile("app/hris_staging/appconfig.json", optional: true, reloadOnChange: true)
//  .Build();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddDbContext<ODC_HRISContext>(options =>
options.UseSqlServer((config["ConnectionStrings:DevConnection"])));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Configuration.AddJsonFile("appconfig.json", optional: true, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<DBMethods>();
builder.Services.AddSwaggerGen(s =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo
    //{
    //    Title = "Authentication",
    //    Version = "v1"
    //});
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

});
builder.Services.AddAuthentication("Basic").AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKey",
        authBuilder =>
        {
            authBuilder.RequireRole("Administrators");
        });

});
builder.Services.AddSingleton<JwtAuthenticationManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Web API");

});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
