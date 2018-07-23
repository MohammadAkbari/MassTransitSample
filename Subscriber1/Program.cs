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
            Console.Title = "Management consumer";
            Console.WriteLine("MANAGEMENT");

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "mycompany.domains.queues.events.mgmt", conf =>
                {
                    //conf.PrefetchCount = 5;

                    conf.Consumer<CustomerRegisteredConsumerMgmt>();
                });
            });
            rabbitBusControl.Start();
            Console.ReadKey();
            rabbitBusControl.Stop();
        }
    }

    public class CustomerRegisteredConsumerMgmt : IConsumer<MessagePublished>
    {
        public Task Consume(ConsumeContext<MessagePublished> context)
        {
            MessagePublished message = context.Message;

            var repository = new Repository();

            var id = repository.Add(message.Text);
            repository.Edit(id);

            Console.WriteLine("A new customer has been registered, congratulations from Management to all parties involved!");
            Console.WriteLine(message.Text);
            return Task.FromResult(context.Message);
        }
    }
}
