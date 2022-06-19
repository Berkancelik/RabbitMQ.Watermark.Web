using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace RabbitMQ.Watermark.Web.Services
{
    public class RabbitMqClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMqClientService> _logger;

        public RabbitMqClientService(ConnectionFactory connectionFactory, IConnection connection, IModel channel, ILogger<RabbitMqClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _connection = connection;
            _channel = channel;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;

            }
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            _channel.QueueDeclare(QueueName, true, false, false, null);
            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWatermark);
            _logger.LogInformation("RabbitMq ile bağlantı kuruldu....");
            return _channel;

        }
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _channel = default;
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile bağlantı koptu....");

        }
    }
}

