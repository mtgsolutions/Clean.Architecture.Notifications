
using Clean.Architecture.Notifications.Api.Infrastructure.Contracts;
using Clean.Architecture.Notifications.Api.Infrastructure.Implementations;
using Clean.Architecture.Notifications.Api.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Clean.Architecture.Notifications.Api.Subscribers;

public class ShippingOrderUpdatedSubscriber : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string Queue = "notifications-service/shipping-order-updated-exchange";
    private const string RoutingKeySubscribers = "shipping-order-updated";
    private readonly IServiceProvider _serviceProvider;
    private const string TrackingsExchange = "notifications-service";

    public ShippingOrderUpdatedSubscriber(IServiceProvider serviceProvider)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = factory.CreateConnection("shipping-order-updated-consumer");
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(TrackingsExchange,"topic",true,false);
        _channel.QueueDeclare(Queue, true, false, false);
        _channel.QueueBind(Queue, TrackingsExchange, RoutingKeySubscribers);

        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var shippingOrderUpdatedEvent = JsonConvert.DeserializeObject<ShippingOrderUpdatedEvent>(message);
            Notify(shippingOrderUpdatedEvent).Wait();

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(Queue, false, consumer);
        return Task.CompletedTask;
    }

    public async Task Notify(ShippingOrderUpdatedEvent @event)
    {
        using var scope = _serviceProvider.CreateScope();

        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var template = new ShippingOrderUpdateTemplate(@event.TrackingNumber, @event.ContactEmail, @event.Description);

        await notificationService.Send(template);
    }
}
