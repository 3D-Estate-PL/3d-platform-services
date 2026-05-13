global using FastEndpoints;
using System.Text.Json.Serialization;
using Azure.Identity;
using BuildingBlocks.Infrastructure.DaprHealthChecks;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using BuildingBlocks.Infrastructure.GoogleSheet;
using BuildingBlocks.Infrastructure.Observability.OpenTelemetry;
using BuildingBlocks.Infrastructure.WebApi.HealthChecks;
using Dapr.Client;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TourStylesConfigurator.Api;
using TourStylesConfigurator.Api.Features;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProducts;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure;
using TourStylesConfigurator.Api.Infrastructure.Email;
using TourStylesConfigurator.Api.Infrastructure.InteriorsDesigners.Products;
using TourStylesConfigurator.Api.Infrastructure.Storage;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

//add this


var builder = WebApplication.CreateBuilder();

var vault = builder.Configuration["KeyVaultName"];
var managedIdentityClientId = builder.Configuration["ManagedIdentityClientId"];
var options = new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = string.IsNullOrEmpty(managedIdentityClientId) ? null: managedIdentityClientId
};

builder.Services.AddSingleton<ConfigurationsSettings>(x => new ConfigurationsSettings
{
    ConfigurationsSheetId = builder.Configuration["ConfigurationsSheetId"]
});

builder.Configuration.AddAzureKeyVault(new Uri($"https://{vault}.vault.azure.net/"),
    new DefaultAzureCredential(options));


builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddDapr(); 



builder.Services.AddDaprClient();
builder.Services.AddLogging();
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(settings =>
{
    settings.SerializerSettings.Converters.Add(new StringEnumConverter
        {NamingStrategy = new CamelCaseNamingStrategy()});
}); //add this

builder.Services.Configure<JsonOptions>(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<IStyleDefaultProductsProvider, StyleDefaultProductsProvider>();
builder.Services.AddScoped<IGetInteriorDesignerProducts, GetInteriorDesignerProducts>();
builder.Services.AddScoped<IGetTourStylesQuery, GetTourStylesQuery>();
builder.Services.AddScoped<IGetRoomTypeCategoriesQuery, GetRoomTypeCategoriesQuery>();
builder.Services.AddScoped<IGetRoomTypesQuery, GetRoomTypesQuery>();
builder.Services.AddScoped<IGetProductCategoriesQuery, GetProductCategoriesQuery>();
builder.Services.AddScoped<IGetProductSubCategoriesQuery, GetProductSubCategoriesQuery>();
builder.Services.AddScoped<ICategoryProvider,CategoryProvider>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.RegisterEmailNotificationContracts(builder.Configuration);

builder.Services.AddSingleton<IInteriorDesignerServiceClient, InteriorDesignerServiceClient>(
    _ => new InteriorDesignerServiceClient(DaprClient.CreateInvokeHttpClient("interior-designers-api")));

builder.Services.AddSingleton<ICoreProductsServiceClient, CoreProductsServiceClient>(
    _ => new CoreProductsServiceClient(DaprClient.CreateInvokeHttpClient("core-products-api")));


builder.AddOpenTelemetryLogging(Consts.AppName);
builder.AddOpenTelemetryMetrics(Consts.AppName);
builder.AddOpenTelemetryTracing(Consts.AppName);


ConfigureDatabase(builder);
ConfigureGoogleSheetClient(builder);

var app = builder.Build();
app.UseAuthorization();
app.UseCustomExceptionHandler(); //add this
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.ResponseBuilder = (failures, context, arg3) =>
    {
        if (failures.Any(x => x.PropertyName.Contains("GeneralErrors")))
        {
            return new
            {
                status = context.Response.StatusCode.ToString(),
                title = failures.FirstOrDefault()?.ErrorMessage,
                errors = Array.Empty<string>()
            };
        }

        return new
        {
            status = context.Response.StatusCode.ToString(),
            title = "Błąd walidacji",
            errors= failures.Select(x => new
            {
                x.PropertyName,
                x.ErrorMessage
            })
        };
    };
});


app.UseOpenApi(); //add this
app.UseSwaggerUi3(s => s.ConfigureDefaults()); //add this
app.AddCoreHealthChecks(Consts.AppName,() => { });
app.Run();


void ConfigureDatabase(WebApplicationBuilder webApplicationBuilder)
{
    
    webApplicationBuilder.Services.AddSingleton(provider =>
    {
        var serviceConfiguration = new ImageStorageSettings();
        var configuration = provider.GetRequiredService<IConfiguration>();
        serviceConfiguration.ImageServiceUrl = configuration["ImageServiceUrl"];
        return serviceConfiguration;
    });

    
    webApplicationBuilder.Services.AddCosmosContext<ToursStylesConfiguratorDbContext,ToursStylesDbConfiguration>(webApplicationBuilder.Configuration);
    webApplicationBuilder.Services.AddScoped<ITourStyleConfigurationRepository, TourStyleConfigurationRepository>();
}

void ConfigureGoogleSheetClient(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.RegisterGoogleSheetClient();
}

