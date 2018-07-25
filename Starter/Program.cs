using Core;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Starter
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        static async Task MainAsync(string[] args)
        {
            Console.Title = "Starter";
            Console.WriteLine($"Starter start at {DateTime.Now}");

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                //cfg.UseInMemoryScheduler();

                //cfg.OverrideDefaultBusEndpointQueueName("Test_Bus");
                cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));
            });

            busControl.Start();

            for (int i = 0; i < 1000000; i++)
            {
                var text = $"message {i}";

                Console.WriteLine($"Send to scheduler {text} at {DateTime.Now}");

                //await busControl.Publish(new MessageStarted
                //{
                //    Text = text
                //});

                await busControl.ScheduleSend(new Uri("rabbitmq://localhost:5672/publisher"), DateTime.Now.AddSeconds(30), new MessageCreated()
                {
                    Text = text
                });

                Thread.Sleep(10000);
            }

            Console.ReadKey();

            busControl.Stop();
        }
    }
}
