using System.Diagnostics;
using BuildingBlocks.Infrastructure.Observability.OpenTelemetry;
using Gateways.PlatformApiGateway;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.AddOpenTelemetryLogging(Consts.AppName);
builder.AddOpenTelemetryMetrics(Consts.AppName);
builder.AddOpenTelemetryTracing(Consts.AppName);
DistributedContextPropagator.Current = DistributedContextPropagator.CreatePassThroughPropagator();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();
app.MapReverseProxy();
app.UseCors(z => z.AllowAnyOrigin().AllowAnyHeader());
app.MapGet("status", context =>
{
    context.Response.WriteAsync("Ready");
    return Task.FromResult(0);
}); 

app.Run();