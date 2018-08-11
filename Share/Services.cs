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
