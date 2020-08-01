using System;
using System.Threading;
using RabbitMQ.Client;
using System.Text;

namespace Ace.Stockroot
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Stockroot",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                int stockUpdateId = 0;
                while (true)
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("Update {0} from Genesis received", stockUpdateId++);
                    string message = "StockUpdate" + stockUpdateId;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "Stockroot",
                        basicProperties: null,
                        body: body);
                }
            }
        }
    }
}