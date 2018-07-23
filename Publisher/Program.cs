using Core;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "This is the customer registration command receiver.";
            Console.WriteLine("CUSTOMER REGISTRATION COMMAND RECEIVER.");

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "mycompany.domains.queues.events.customer", conf =>
                {
                    conf.Consumer<RegisterCustomerConsumer>();
                });
            });

            rabbitBusControl.Start();

            for (int i = 0; i < 100000000; i++)
            {
                rabbitBusControl.Publish(new MessageCreated
                {
                    Text = $"message {i}"
                });

                Thread.Sleep(10);
            }

            Console.ReadKey();

            rabbitBusControl.Stop();
        }
    }

    public class RegisterCustomerConsumer : IConsumer<MessageCreated>
    {
        public Task Consume(ConsumeContext<MessageCreated> context)
        {
            MessageCreated message = context.Message;
            Console.WriteLine("A new customer has signed up, it's time to register it in the command receiver. Details: ");
            Console.WriteLine(message.Text);

            context.Publish(new MessagePublished
            {
                Text = message.Text,
            });

            return Task.FromResult(context.Message);
        }
    }
}
