using Core;
using MassTransit;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Starter2
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
            var busControl = Bus.Factory.CreateUsingRabbitMq(async cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });

                //cfg.UseInMemoryScheduler();

                //cfg.UseDelayedExchangeMessageScheduler();

                //var scheduler = await CreateSchedulerAsync();

                //cfg.ReceiveEndpoint("quartz", endpoint =>
                //{
                //    endpoint.Consumer(() => new ScheduleMessageConsumer(scheduler));
                //    endpoint.Consumer(() => new CancelScheduledMessageConsumer(scheduler));
                //});

                cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));

                cfg.ReceiveEndpoint(host, "publisher", conf =>
                {
                    conf.Consumer<PublisherConsumer>();
                });

                cfg.ReceiveEndpoint(host, "subscriber", conf =>
                {
                    conf.Consumer<SubscriberConsumer>();
                });
            });

            busControl.Start();

            for (int i = 0; i < 1000000; i++)
            {
                var text = $"message {i}";

                Console.WriteLine($"Schedule: {text}");

                var scheduledMessage = await busControl.ScheduleSend(new Uri("rabbitmq://localhost/publisher"),
                    DateTime.Now.AddSeconds(5),
                    new ScheduleMessage()
                    {
                        Text = text
                    });

                Thread.Sleep(2000);
            }

            Console.ReadKey();

            busControl.Stop();
        }

        static async Task<IScheduler> CreateSchedulerAsync()
        {
            var schedulerFactory = new StdSchedulerFactory();

            var scheduler = await schedulerFactory.GetScheduler();

            return scheduler;
        }
    }
}
