using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ace.Sequencer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "SnapshotSequencer",
                    durable: false,
                    exclusive: false,
                    autoDelete: true,
                    arguments: null);
                channel.QueueBind(queue: "SnapshotSequencer",exchange: "Snapshot",routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = Encoding.UTF8.GetString(ea.Body);
                    Sequence(body);
                };
                channel.BasicConsume(queue: "SnapshotSequencer",
                    autoAck: true,
                    consumer: consumer);

                Console.ReadLine();
            }
        }

        public static void Sequence(string body)
        {
            Console.WriteLine("Sequencing snapshot " + body);
        }
    }
}
