
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using testTask.Data;
using testTask.Models;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
// Add services to the container.
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

//});
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Debug().WriteTo.File("logs/logging.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}  );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
