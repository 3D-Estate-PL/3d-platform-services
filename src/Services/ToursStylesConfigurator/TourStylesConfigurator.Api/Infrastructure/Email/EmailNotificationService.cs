using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using TourStylesConfigurator.Api.Features;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Infrastructure.Email;

public class EmailNotificationService : IEmailNotificationService 
{
    private readonly EmailNotificationClient _emailNotificationClient;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(EmailNotificationClient emailNotificationClient, ILogger<EmailNotificationService> logger)
    {
        _emailNotificationClient = emailNotificationClient;
        _logger = logger;
    }

    public async Task SendEmailAsync(SendEmailNotificationCommand command,  CancellationToken cancellationToken)
    {
        await _emailNotificationClient.SendMessageAsync(command, cancellationToken);
    }

    public async Task NewConfigurationEmailAsync(TourStyleConfiguration styleConfiguration, ConfigurationOwner owner,
        string envName,CancellationToken c)
    {
        _logger.LogInformation("Preparing email {EnvironmentName}", envName);

        try
        {
            var sendEmailNotificationCommand = new SendEmailNotificationCommand
            {
                ContentTemplate = new ContentTemplate
                {
                    TemplateUrl =
                        "https://3dnotificationstaging.blob.core.windows.net/email-templates/email-template-start.html",
                    Parameters = new Dictionary<string, string>
                    {
                        {
                            "CODE", styleConfiguration.Code
                        },
                        {"CONFIGURATION_LINK",$"https://{InteriorDesignerContextProvider.GetHostName(styleConfiguration.InteriorDesignerCode,
                            envName)}.3destate.pl"}
                    }
                },
                Recipients = owner.Email,
                SourceServiceCode = "TourConfiguration",
                NotificationId = Guid.NewGuid(),
                SenderEmail = "system@3destate.pl",
                SenderName = "3D Estate Configurator",
                Title = $"Konfiguracja {styleConfiguration.Code} rozpoczęta.",
            };
            await SendEmailAsync(sendEmailNotificationCommand, c);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}


public class EmailNotificationClient
{
    private readonly QueueClient _queue;

    public EmailNotificationClient(QueueClient queue)
    {
        _queue = queue;
    }

    public async Task SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
    {
        await _queue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        await _queue.SendMessageAsync(ToJson(message), cancellationToken);
    }

    private string ToJson<TMessage>(TMessage message)
    {
        return Base64Encode(JsonSerializer.Serialize(message));
    }

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
}