using Core;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Starter2
{
    public class PublisherConsumer : IConsumer<ScheduleMessage>
    {
        public Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            Console.WriteLine($"In Publisher: {context.Message.Text}");

            context.Publish(new PublishMessage
            {
                Text = context.Message.Text,
            });

            return Task.FromResult(context.Message);
        }
    }
}
