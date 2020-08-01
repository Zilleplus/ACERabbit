using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Ace.Snapshot
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("Snapshot", "fanout");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => handleStockRoot(channel, ea.Body);
                channel.BasicConsume(queue: "Stockroot",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Snapshot started:");
                Console.ReadLine();
            }
        }

        public static void handleStockRoot(IModel channel, byte[] body)
        {
            var stockRoot = Encoding.UTF8.GetString(body);
            Console.WriteLine("Receive Stockroot {0} -> Building snapshot", stockRoot);

            channel.BasicPublish(exchange: "Snapshot",
                routingKey: "Snapshot",
                basicProperties: null,
                body: body);
        }
    }
}
