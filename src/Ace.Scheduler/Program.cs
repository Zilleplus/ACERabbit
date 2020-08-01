using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace Ace.Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var arguments = new Dictionary<string, object>
                {
                   { "x-max-length", 1 },
                   { "x-overflow", "drop-head" }
                };

                var response = channel.QueueDeclare(queue: "SnapshotScheduler",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments);
                channel.QueueBind(queue: "SnapshotScheduler", exchange: "Snapshot", routingKey: "");

                while (true)
                {
                    var data = channel.BasicGet("SnapshotScheduler", false);
                    while (data == null)
                    {
                        data = channel.BasicGet("SnapshotScheduler", true); // keep polling
                    }
                    var message = Encoding.UTF8.GetString(data.Body);
                    Schedule(message);
                }
            }
        }

        public static void Schedule(string body)
        {
            Console.WriteLine("Scheduling snapshot: " + body);
            Thread.Sleep(5000);
        }
    }
}
