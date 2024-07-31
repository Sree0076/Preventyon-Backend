using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Preventyon;
using Preventyon.Data;
using Preventyon.EndPoints;
using Preventyon.Models;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;
using Preventyon.Service;
using Preventyon.Service.IService;
using Serilog;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Preventyon API", Version = "v1" });
});

/*###############################################  SERILOGGRR #######################################################################*/
var logerConfig = builder.Configuration.GetSection("Serilog");
Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(logerConfig)
           .WriteTo.Console()
           .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

builder.Host.UseSerilog()  // Integrate Serilog into the application
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
            });

/*#########################################################################################################################*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped< IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAssignedIncidentRepository, AssignedIncidentRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped< EmployeeRepository>();
// Register Services
builder.Services.AddScoped<IAssignedIncidentService, AssignedIncidentService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IEmployeeService,EmployeeService>();
builder.Services.AddScoped <IEmailService, EmailService>();
builder.Services.AddScoped<IEmailRepository,EmailRepository>();

var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
builder.Services.AddFluentEmail(smtpSettings.FromEmail, smtpSettings.FromName)
.AddRazorRenderer()
.AddSmtpSender(new SmtpClient(smtpSettings.Host)
{
    Port = smtpSettings.Port,
    Credentials = new System.Net.NetworkCredential(smtpSettings.UserName, smtpSettings.Password),
    EnableSsl = true,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    UseDefaultCredentials = false,
});




builder.Services.AddDbContext<ApiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureEndPoints();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "images"))
,
    RequestPath = "/images"
});


app.MapControllers();

app.Run();
