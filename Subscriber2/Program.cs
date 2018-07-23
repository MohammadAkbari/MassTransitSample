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
            Console.Title = "Sales consumer";
            Console.WriteLine("SALES");
            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "mycompany.domains.queues.events.sales", conf =>
                {
                    conf.Consumer<CustomerRegisteredConsumerSls>();
                });
            });

            rabbitBusControl.Start();
            Console.ReadKey();
            rabbitBusControl.Stop();
        }
    }

    public class CustomerRegisteredConsumerSls : IConsumer<MessagePublished>
    {
        public Task Consume(ConsumeContext<MessagePublished> context)
        {
            MessagePublished message = context.Message;

            var repository = new Repository();

            var id = repository.Add(message.Text);
            repository.Edit(id);

            Console.WriteLine("Great to see the new customer finally being registered, a big sigh from sales!");
            Console.WriteLine(message.Text);
            return Task.FromResult(context.Message);
        }
    }
}
