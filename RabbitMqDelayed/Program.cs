using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMqDelayed
{
    class Program
    {
        const string Exchange = "Exchange2";
        const string Queue = "Queue2";
        const string Route = "route2";


        static void Main(string[] args)
        {
            for (int i = 0; i < 10000; i++)
            {
                if(i%2 == 0)
                {
                    Send($"message {i} scheduled at {DateTime.Now}", 3000);
                }
                else
                {
                    Send($"message {i} scheduled at {DateTime.Now}", 10000);
                }

                Thread.Sleep(5000);
            }

            Console.ReadKey();
        }

        public static void Send(string message, int milliSecond)
        {
            var args = new Dictionary<string, object>
            {
                {"x-delayed-type", "direct"}
            };

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, "x-delayed-message", true, false, args);

                    var queue = channel.QueueDeclare(Queue, true, false, false, null);

                    channel.QueueBind(queue, Exchange, Route);

                    var props = channel.CreateBasicProperties();
                    props.Headers = new Dictionary<string, object>
                    {
                        {"x-delay", milliSecond}
                    };

                    channel.BasicPublish(Exchange, Route, props, Encoding.Default.GetBytes(message));

                    Console.WriteLine($"{message}");
                }
            }
        }
    }
}
