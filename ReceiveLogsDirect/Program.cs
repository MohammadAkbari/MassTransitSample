using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Share;
using System;
using System.Text;

namespace ReceiveLogsDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = Services.ConnectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                    var queueName = channel.QueueDeclare().QueueName;

                    if (args.Length < 1)
                    {
                        Console.Error.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]} [info] [warning] [error]");
                        Console.WriteLine("Press [enter] to exit.");
                        Console.ReadLine();

                        return;
                    }
                            
                    foreach (var severity in args)
                    {
                        channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: severity);
                    }

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine($"[x] Received '{routingKey}':'{message}'");
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();

                    return;
                }
            }
        }
    }
}
