using Core;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Threading.Tasks;

namespace Subscriber1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Subscriber1";
            Console.WriteLine($"Subscriber1 start at {DateTime.Now}");

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "masstransitsample.queues.events.subscriber1", conf =>
                {
                    //conf.PrefetchCount = 5;

                    conf.Consumer<Consumer>();
                });
            });
            rabbitBusControl.Start();
            Console.ReadKey();
            rabbitBusControl.Stop();
        }
    }

    public class Consumer : IConsumer<MessagePublished>
    {
        public Task Consume(ConsumeContext<MessagePublished> context)
        {
            MessagePublished message = context.Message;
            Console.WriteLine($"Get {message.GetType()} at {DateTime.Now}");

            var repository = new Repository();

            var id = repository.Add(message.Text);
            repository.Edit(id);

            Console.WriteLine(message.Text);
            return Task.FromResult(context.Message);
        }
    }
}
