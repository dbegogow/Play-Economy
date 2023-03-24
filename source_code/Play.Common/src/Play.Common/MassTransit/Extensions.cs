using System.Reflection;
using Play.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit;

namespace Play.Common.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransiteWithRabbitMq(this IServiceCollection services)
        => services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();

                    var rabbitMQSettings = configuration
                        .GetSection(nameof(RabbitMQSettings))
                        .Get<RabbitMQSettings>();

                    var serviceSettings = configuration
                        .GetSection(nameof(ServiceSettings))
                        .Get<ServiceSettings>();

                    configurator.Host(rabbitMQSettings.Host);

                    configurator.ConfigureEndpoints(
                        context,
                        new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

                    configurator.UseMessageRetry(
                        retryConfigurator => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });
}
