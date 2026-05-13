using System.Reflection;
using BuildingBlocks.Abstractions.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.GoogleSheet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterGoogleSheetClient(this IServiceCollection services)
    {
        services.AddScoped<GoogleSheetsClient>();
        services.AddScoped<IExcelDictionaryProvider, GoogleDictionariesProvider>();

        return services;
    }
}