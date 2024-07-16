using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Preventyon;
using Preventyon.Data;
using Preventyon.EndPoints;
using Preventyon.Repository;
using Preventyon.Repository.IRepository;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IncidentRepository>();
builder.Services.AddScoped< EmployeeRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseNpgsql("Host=preventyonserver.postgres.database.azure.com;Database=Preventyon;Username=preventyon;Password=root@2024"));
var app = builder.Build();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureEndPoints();
app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
