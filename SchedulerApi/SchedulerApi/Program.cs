using System.Data;
using System.Text.Json.Serialization;
using FluentValidation;
using Npgsql;
using SchedulerApi.Controllers;
using SchedulerApi.Domain;
using SchedulerApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
{
	x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IReadScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IAvalableBookingsService, AvalableBookingsService>();
builder.Services.AddTransient<IValidator<GetAvailableManagersRequest>, GetAvailableManagersRequestValidator>();

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
