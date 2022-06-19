using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Watermark.Web.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMqClientService _rabbitMqClientService;

        public RabbitMQPublisher(RabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }

        public void Publish(ProdcutImageCreatedEvent prodcutImageCreatedEvent)
        {
            var channel = _rabbitMqClientService.Connect();

            var bodyString = JsonSerializer.Serialize(prodcutImageCreatedEvent);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();

            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMqClientService.ExchangeName, routingKey: RabbitMqClientService.RoutingWatermark,basicProperties: properties,body: bodyByte);
        }
    }
}
