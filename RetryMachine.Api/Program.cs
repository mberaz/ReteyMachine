using Microsoft.EntityFrameworkCore;
using RetryMachine;
using RetryMachine.Api.Actions;
using RetryMachine.SQL.Models;
using RetryMachine.SQL.Repositories;
using RetryMachine.SQL.Storage;
using RetryStorage = RetryMachine.Api.Storage.RetryStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRetryable, AutoLogAction>();
builder.Services.AddScoped<IRetryable, UserActionLogAction>();

builder.Services.AddScoped<IRetryMachine, RetryMachineRunner>();
builder.Services.AddScoped<IRetryStorage, RetryStorage>();
builder.Services.AddScoped<IRetryTaskRepository, RetryTaskRepository>();

var connectionString = "";

builder.Services.AddDbContext<RetrymachineContext>(options =>
options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();