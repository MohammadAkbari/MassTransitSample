using Core;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Starter2
{
    public class SubscriberConsumer : IConsumer<PublishMessage>
    {
        public Task Consume(ConsumeContext<PublishMessage> context)
        {
            Console.WriteLine($"In Subscriber: {context.Message.Text}");

            return Task.FromResult(context.Message);
        }
    }
}
