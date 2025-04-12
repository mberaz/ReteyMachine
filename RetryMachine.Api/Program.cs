using RetryMachine;
using RetryMachine.Api.Actions;
using RetryMachine.Api.Service;
using RetryMachine.Api.Sevices;
using RetryMachine.Api.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRetryable, LogAction>();
builder.Services.AddScoped<IRetryable, MonitoringAction>();

builder.Services.AddSingleton(new RetrySettings { DelayInSeconds = 30 });
builder.Services.AddScoped<IRetryMachineRunner, RetryMachineRunner>();

builder.Services.AddScoped<IRetryStorage, AzureQueueStorage>();

builder.Services.AddScoped<IRandomService, RandomService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();

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