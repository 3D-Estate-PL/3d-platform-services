using Ben.Diagnostics;
using BuildingBlocks.Infrastructure.DaprHealthChecks;
using BuildingBlocks.Infrastructure.Observability.OpenTelemetry;
using BuildingBlocks.Infrastructure.WebApi.HealthChecks;
using ImagesManagement.Api;
using ImagesManagement.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

//add this

var builder = WebApplication.CreateBuilder();


builder.AddOpenTelemetryLogging(Consts.AppName);
builder.AddOpenTelemetryMetrics(Consts.AppName);
builder.AddOpenTelemetryTracing(Consts.AppName);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = $"Platform - {Consts.AppName}", Version = "v1"});
    c.CustomSchemaIds(x=>x.DeclaringType != null? x.DeclaringType.Name : x.Name );
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddDapr();

builder.Services.AddControllers().AddDapr();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.EnvironmentName == "dev");


var app = builder.Build();

if (builder.Environment.EnvironmentName == "dev")
{
    app.UseBlockingDetection();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Consts.AppName} V1");
});
app.UseCloudEvents();
app.MapDefaultControllerRoute();
app.MapControllers();
app.RegisterIntegrationEventSubscriptions();


app.AddCoreHealthChecks(Consts.AppName,() => { });