using SecretSanta.Services;

namespace SecretSanta.Extensions
{
    public static class ServicesConfigurationExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISecretSantaService, SecretSantaService>();

            return services;
        }
    }
}
