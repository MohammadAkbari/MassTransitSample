using Core;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Threading.Tasks;

namespace Subscriber2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Subscriber2";
            Console.WriteLine($"Subscriber2 start at {DateTime.Now}");

            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "masstransitsample.queues.events.subscriber2", conf =>
                {
                    conf.Consumer<Consumer>();
                });
            });

            busControl.Start();
            Console.ReadKey();
            busControl.Stop();
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
