using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IEmailNotificationService
{
    Task SendEmailAsync(SendEmailNotificationCommand command, CancellationToken cancellationToken);

    Task NewConfigurationEmailAsync(TourStyleConfiguration styleConfiguration, ConfigurationOwner owner,
        string envName,CancellationToken c);
}


public enum ChannelType
{
    Email
}

public record struct SendEmailNotificationCommand
{
    public ChannelType ChannelType => ChannelType.Email;
    public required string SourceServiceCode { get; init; }
    public required string Recipients { get; init; }
    public required string Title { get; init; }
    public string Content { get; init; }

    public required string SenderEmail { get; set; }

    public required string SenderName { get; set; }
    
    public string Bcc => "configurator-orders@3destate.pl";

    public ContentTemplate ContentTemplate { get; set; }

    public required Guid NotificationId { get; init; }
    public Attachment? Attachment { get; set; }
}

public class ContentTemplate
{
    public required string TemplateUrl { get; set; }
    public required Dictionary<string, string> Parameters { get; set; }
}

public class Attachment
{
    public required string FileName { get; init; }
    public required string FileUrl { get; init; }
}