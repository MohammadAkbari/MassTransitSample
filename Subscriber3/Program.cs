using Core;
using MassTransit;
using System;
using System.Threading.Tasks;
using Topshelf;

namespace Subscriber3
{
    class Program
    {
        public static int Main(string[] args)
        {
            return (int)HostFactory.Run(cfg => cfg.Service(x => new EventConsumerService()));
        }
    }

    public class EventConsumerService : ServiceControl
    {
        IBusControl _bus;

        public bool Start(HostControl hostControl)
        {
            Console.WriteLine("Start **********************************************");

            _bus = ConfigureBus();
            _bus.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Console.WriteLine("Stop **********************************************");

            _bus?.Stop(TimeSpan.FromSeconds(30));

            return true;
        }

        IBusControl ConfigureBus()
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var host = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(host, "masstransitsample.queues.events.subscriber3", conf =>
                {
                    conf.Consumer<Consumer>();
                });
            });

            return busControl;
        }
    }

    public class Consumer : IConsumer<MessagePublished>
    {
        public Task Consume(ConsumeContext<MessagePublished> context)
        {
            MessagePublished message = context.Message;

            Console.WriteLine($"Get {message.GetType()} at {DateTime.Now}");

            Console.WriteLine(message.Text);

            return Task.FromResult(context.Message);
        }
    }
}
