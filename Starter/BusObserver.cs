using MassTransit;
using System;
using System.Threading.Tasks;

namespace Starter
{
    public class BusObserver : IBusObserver
    {
        public Task CreateFaulted(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task PostCreate(IBus bus)
        {
            throw new NotImplementedException();
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            throw new NotImplementedException();
        }

        public Task PostStop(IBus bus)
        {
            throw new NotImplementedException();
        }

        public Task PreStart(IBus bus)
        {
            throw new NotImplementedException();
        }

        public Task PreStop(IBus bus)
        {
            throw new NotImplementedException();
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
