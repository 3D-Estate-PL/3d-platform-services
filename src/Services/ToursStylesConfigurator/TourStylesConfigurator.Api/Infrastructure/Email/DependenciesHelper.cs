namespace TourStylesConfigurator.Api.Infrastructure.Email;

public static class DependenciesHelper
{
    public static IServiceCollection AddConfiguration<TConfigurationType>(this IServiceCollection services, IConfiguration configuration)
        where TConfigurationType : class, IServiceConfiguration, new()
    {
        services.AddSingleton(provider => {
                
            var serviceConfiguration = new TConfigurationType();
            configuration.Bind(typeof(TConfigurationType).Name, serviceConfiguration);

            var validationErrors = serviceConfiguration.Validate().ToList();
            if (validationErrors.Any())
            {
                throw new ArgumentException($"Invalid configuration for {typeof(TConfigurationType).Name}. {string.Join(',', validationErrors)}");
            }

            return serviceConfiguration;
        });
        return services;
    }
}