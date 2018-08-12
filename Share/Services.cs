using RabbitMQ.Client;

namespace Share
{
    public static class Services
    {
        public static ConnectionFactory ConnectionFactory
        {
            get
            {
                var factory = new ConnectionFactory()
                {
                    //Uri = new Uri(@"amqps://rumacvwx:ELu2aFDH_h0lrx1UoW1rrUg7BrqkR6t1@baboon.rmq.cloudamqp.com/rumacvwx"),
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest"
                };

                return factory;
            }
        }
    }
}
