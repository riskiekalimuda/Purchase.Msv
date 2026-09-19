using MassTransit;
using MessageMQCommon.MQ.Names;
using MessageMQCommon.Parameters;
using Purchase.Msv.Consumers;
using Purchase.Msv.Models;

namespace Purchase.Msv.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQSetting = configuration.GetSection("RabbitMqSettings").Get<RabbitMQParameter>() ?? new RabbitMQParameter();

            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<PurchaseMsvDbContext>(o =>
                {
                    o.UsePostgres();
                    o.UseBusOutbox();
                    o.QueryDelay = TimeSpan.FromSeconds(10);
                });

                x.AddConsumersFromNamespaceContaining<PurchaseCreatedResultConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMQSetting.Host, rabbitMQSetting.VirtualHost, h =>
                    {
                        h.Username(rabbitMQSetting.Username);
                        h.Password(rabbitMQSetting.Password);
                    });

                    cfg.ReceiveEndpoint(QueueNames.PurchaseQueue.PurchaseCreatedResultQueue, e =>
                    {
                        e.Durable = true;
                        e.UseMessageRetry(r => r.Interval(20, 10));
                        e.ConfigureConsumer<PurchaseCreatedResultConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
