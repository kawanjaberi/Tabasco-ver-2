using Microsoft.OpenApi.Models;
using Serilog;
using Tabasco.Services;

var builder = WebApplication.CreateBuilder(args);

// Config Logger
var logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
logger.Information("Starting TABASCO");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
/// Configure Swagger.
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Kavenegar SMS API",
		Version = "v1",
		Description = "An API to send SMS using the Kavenegar service",
	});
});
var apiKey = Environment.GetEnvironmentVariable("KAVEHNEGAR_API_KEY");
builder.Services.AddScoped<ISmsService>(provider =>
{
	if (apiKey != null)
	{
		var logger 	= provider.GetRequiredService<ILogger<SmsService>>();
		return new SmsService(apiKey, logger);
	}
	else
	{
		return null;
	}
});
// HealthCheck Kavenegar service:
builder.Services.AddScoped<IHealthCheckService>(provider =>
{
	if (apiKey != null)
	{
		var logger	= provider.GetRequiredService<ILogger<HealthCheckService>>();
		return new HealthCheckService(apiKey, logger);
	}
	else
	{
		return null;
	}
}
);


// Configure logging
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
// builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

// builder.Host.UseSerilog(Log.logger);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();