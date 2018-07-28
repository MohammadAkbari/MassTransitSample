using Core;
using MassTransit;
using MassTransit.QuartzIntegration;
using Quartz;
using Quartz.Impl;
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

        private static async Task<IScheduler> CreateSchedulerAsync()
        {
            var schedulerFactory = new StdSchedulerFactory();

            var scheduler = await schedulerFactory.GetScheduler();

            return scheduler;
        }

        static async Task MainAsync(string[] args)
        {
            Console.Title = "Starter";
            Console.WriteLine($"Starter start at {DateTime.Now}");

            var busControl = Bus.Factory.CreateUsingRabbitMq(async cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                //cfg.UseInMemoryScheduler();
                //cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));

                var scheduler = await CreateSchedulerAsync();
                cfg.ReceiveEndpoint("quartz", e =>
                {
                    cfg.UseMessageScheduler(e.InputAddress);

                    e.Consumer(() => new ScheduleMessageConsumer(scheduler));
                    e.Consumer(() => new CancelScheduledMessageConsumer(scheduler));
                });


                cfg.ReceiveEndpoint(host, "publisher1", conf =>
                {
                    conf.Consumer<Consumer>();
                });
            });

            busControl.Start();

            var sendEndpoint = await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/quartz"));

            for (int i = 0; i < 1000000; i++)
            {
                var text = $"message {i}";

                Console.WriteLine($"Send to scheduler {text} at {DateTime.Now}");

                //await busControl.Publish(new MessageStarted
                //{
                //    Text = text
                //});

                await sendEndpoint.ScheduleSend(new Uri("rabbitmq://localhost/publisher"), DateTime.Now.AddSeconds(30), new MessageCreated()
                {
                    Text = text
                });

                Thread.Sleep(10000);
            }

            Console.ReadKey();

            busControl.Stop();
        }
    }

    public class Consumer : IConsumer<MessageCreated>
    {
        public Task Consume(ConsumeContext<MessageCreated> context)
        {
            MessageCreated message = context.Message;
            Console.WriteLine($"Get {message.GetType()} at {DateTime.Now}");
            Console.WriteLine(message.Text);

            context.Publish(new MessagePublished
            {
                Text = message.Text,
            });

            return Task.FromResult(context.Message);
        }
    }
}
