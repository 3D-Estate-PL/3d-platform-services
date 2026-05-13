
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;

namespace BuildingBlocks.Infrastructure.Observability.OpenTelemetry;

public static class ServiceExtensions
{
    public static WebApplicationBuilder AddOpenTelemetryMetrics(this WebApplicationBuilder builder, string appName)
    {
        var grpcUrl = GetGrpcEndpointUri(builder.Configuration);
        var resourceBuilder = CreateResourceBuilder(builder.Configuration, appName);
        var serviceCollection = builder.Services;
        
        // Configure metrics
        serviceCollection.AddOpenTelemetry().WithMetrics(config =>
        {
            config
                .AddConsoleExporter()
                .AddOtlpExporter(conf =>
                {
                    conf.Protocol = OtlpExportProtocol.Grpc;
                    conf.Endpoint = grpcUrl;
                })
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .SetResourceBuilder(resourceBuilder);
        });

        return builder;
    }

    public static WebApplicationBuilder AddOpenTelemetryLogging(this WebApplicationBuilder builder,string appName)
    {
        var grpcUrl = GetGrpcEndpointUri(builder.Configuration);
        var resourceBuilder = CreateResourceBuilder(builder.Configuration, appName);
        builder.Services.AddSingleton(TracerProvider.Default.GetTracer(appName));
        Log.Logger = new LoggerConfiguration()
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = grpcUrl.ToString();
                    options.Protocol = OtlpProtocol.Grpc;
                })
                .Enrich.WithEnvironmentName()
                .Enrich.WithSpan()
                .Enrich.WithProperty("ApplicationName", appName)
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            
        builder.Services.AddLogging();
       // builder.Host.UseSerilog();
        builder.Logging.AddOpenTelemetry(config =>
        {

            config.AddConsoleExporter()
                .AddOtlpExporter(conf =>
                {
                    conf.Protocol = OtlpExportProtocol.Grpc;
                    conf.Endpoint = grpcUrl;
                })
                .SetResourceBuilder(resourceBuilder);
            config.IncludeScopes = true;
            config.ParseStateValues = true;
            config.IncludeFormattedMessage = true;
        });

        return builder;
    }



    public static WebApplicationBuilder AddOpenTelemetryTracing(this WebApplicationBuilder builder, string appName)
    {
        var grpcUrl = GetGrpcEndpointUri(builder.Configuration);
        var resourceBuilder = CreateResourceBuilder(builder.Configuration, appName);
        var serviceCollection = builder.Services;
        
        // Configure important OpenTelemetry tracing settings, the console exporter, and instrumentation library
        serviceCollection.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
            .AddConsoleExporter()
                .AddOtlpExporter(conf =>
                {
                    conf.Protocol = OtlpExportProtocol.Grpc;
                    conf.Endpoint = grpcUrl;
                })
                .AddHttpClientInstrumentation(x=>x.RecordException = true)
                .AddAspNetCoreInstrumentation(x=>x.RecordException = true)
                .SetResourceBuilder(resourceBuilder);
        });
        return builder;
    }

    private static ResourceBuilder CreateResourceBuilder(IConfiguration configuration, string appName)
    {
        var serviceNamespace = configuration["ServiceNamespace"];
        var serviceVersion = configuration["ServiceVersion"];
        var serviceName = $@"{appName}-{configuration["Environment"]}";
        var resourceAttributes = new Dictionary<string, object>();


        var resourceBuilder = ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector()
            .AddService(serviceName: serviceName, serviceNamespace: @serviceNamespace, serviceVersion: serviceVersion)
            .AddAttributes(resourceAttributes);
        return resourceBuilder;
    }

    private static Uri GetGrpcEndpointUri(IConfiguration configuration)
    {
        var otlServerUrl = configuration["OtlGrpcEndpoint"] ??
                           throw new ArgumentException("OtlGrpcEndpoint parameter is required.");
        var grpcUrl = new Uri(otlServerUrl);
        return grpcUrl;
    }
}