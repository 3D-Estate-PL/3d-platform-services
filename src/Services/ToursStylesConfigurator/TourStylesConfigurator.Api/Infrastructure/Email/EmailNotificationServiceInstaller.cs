using Azure.Storage.Queues;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Infrastructure.Email;

public static class EmailNotificationServiceInstaller
{
    public static IServiceCollection RegisterEmailNotificationContracts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguration<EmailNotificationServiceConfiguration>(configuration);
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddSingleton(provider => {

            var serviceConfiguration = provider.GetRequiredService<EmailNotificationServiceConfiguration>();
            var queueClient = new QueueClient(serviceConfiguration.ConnectionString, serviceConfiguration.QueueName);
            return new EmailNotificationClient(queueClient);
        });

        return services;
    }
}

public class EmailNotificationServiceConfiguration : IServiceConfiguration
{
    public string ConnectionString { get; set; }
    public string QueueName =>  "send-email-notification-commands";
    
    public IEnumerable<string> Validate()
    {
        if (string.IsNullOrWhiteSpace(QueueName))
        {
            yield return "QueueName can not be null or empty";
        }
    }
}

public interface IServiceConfiguration
{
    IEnumerable<string> Validate();
}