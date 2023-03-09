namespace MassTransitTaskLeak.Common.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using MassTransitTaskLeak.Common.Messages;

    public class ConsumerRequest : IConsumer<TestRequest>
    {
        private readonly Helper _helper;

        public ConsumerRequest(Helper helper)
        {
            _helper = helper;
        }

        public Task Consume(ConsumeContext<TestRequest> context)
        {
            Console.WriteLine(
                $"#### Put a break point here after few requests to check the number of Tasks!!!");
            Console.WriteLine(
                $"ThreadPool queued tasks: Completed:{ThreadPool.CompletedWorkItemCount} Pending:{ThreadPool.PendingWorkItemCount} Threads:{ThreadPool.ThreadCount}");
            var responseQueue = context.Message.ResponseQueue;
            return _helper.SendToAsync(responseQueue, new TestResponse(responseQueue));
        }
    }
}