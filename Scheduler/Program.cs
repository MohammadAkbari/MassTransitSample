using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;
using Topshelf;

namespace Scheduler
{
    class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(cfg => cfg.Service(x => new EventConsumerService()));

            //MainAsync(args).GetAwaiter().GetResult();
            //Console.ReadKey();
        }

        static async Task MainAsync(string[] args)
        {
            Scheduler sc = new Scheduler();
            await sc.Start(DateTime.Now.AddMinutes(1));
        }
    }

    public class EventConsumerService : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            Scheduler sc = new Scheduler();
            sc.Start(DateTime.Now.AddMinutes(1)).GetAwaiter().GetResult();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }

    public class Scheduler
    {
        public async Task Start(DateTime date)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            await scheduler.Start();

            var job = JobBuilder.Create<Job>().Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Scheduler", "SchedulerGroup")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                //.StartAt(date)
                .StartNow()
                .WithPriority(1)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }

    public class Job : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"Execute at {DateTime.Now}");

            return Task.FromResult(0);
        }
    }
}
