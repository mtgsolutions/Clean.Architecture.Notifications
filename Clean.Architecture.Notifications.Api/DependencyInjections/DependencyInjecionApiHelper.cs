using Clean.Architecture.Notifications.Api.Infrastructure.Contracts;
using Clean.Architecture.Notifications.Api.Infrastructure.Implementations;
using Clean.Architecture.Notifications.Api.Options;
using Clean.Architecture.Notifications.Api.Subscribers;
using SendGrid.Extensions.DependencyInjection;

namespace Clean.Architecture.Notifications.Api.DependencyInjections
{
    public static class DependencyInjecionApiHelper
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddEmailService(configuration)
                .AddNotifications()
                .AddRabbitMq(configuration)
                .AddHostedService();

            return services;
        }

        private static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSendGrid(options =>
            {
                options.ApiKey = configuration.GetSection("SendGrid:ApiKey").Value;
            });

            return services;
        }

        private static IServiceCollection AddNotifications(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, EmailService>();

            return services;
        }

        private static IServiceCollection AddHostedService(this IServiceCollection services)
        {
            services.AddHostedService<ShippingOrderUpdatedSubscriber>();

            return services;
        }

        private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(options => configuration.GetSection("RabbitMq").Bind(options));

            return services;
        }
    }
}
