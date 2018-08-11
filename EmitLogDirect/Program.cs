using RabbitMQ.Client;
using Share;
using System;
using System.Linq;
using System.Text;

namespace EmitLogDirect
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

                    var severity = (args.Length > 0) ? args[0] : "info";

                    var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "direct_logs", routingKey: severity, basicProperties: null, body: body);

                    Console.WriteLine($"[x] Sent '{severity}':'{message}'");
                }
            }

            Console.WriteLine(" Press [enter] to exit.");

            Console.ReadLine();
        }
    }
}
