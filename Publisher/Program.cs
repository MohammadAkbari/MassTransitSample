using Core;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Publisher";
            Console.WriteLine($"Publisher start at {DateTime.Now}");

            var busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var host = rabbit.Host(new Uri("rabbitmq://localhost:5672"), settings =>
                {
                    settings.Password("ninisite");
                    settings.Username("ninisite");
                });

                rabbit.ReceiveEndpoint(host, "publisher", conf =>
                {
                    conf.Consumer<Consumer>();
                });
            });

            busControl.Start();
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
