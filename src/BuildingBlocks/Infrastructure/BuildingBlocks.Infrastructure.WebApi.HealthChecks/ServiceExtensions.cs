using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.WebApi.HealthChecks;

public static class ServiceExtensions
{
    
        public static IApplicationBuilder AddCoreHealthChecks(this WebApplication app, string appName,
            Action onCloseAppAction)
        {
            app.MapHealthChecks("/healthz/startup", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/healthz/readiness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
            app.MapHealthChecks("/healthz/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });            
            try
            {
                app.Logger.LogInformation("Starting web host ({ApplicationName})...",appName);
                app.Run();
            }
            catch (Exception ex)
            {
                app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", appName);
            }
            finally
            {
                onCloseAppAction();
            }

            return app;
        }
    
}