namespace MassTransitTaskLeak.Common.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;
    using MassTransitTaskLeak.Common.Messages;

    public class ConsumerResponse : IConsumer<TestResponse>
    {
        public static readonly AutoResetEvent ResetEvent = new AutoResetEvent(false);

        public Task Consume(ConsumeContext<TestResponse> context)
        {
            Console.WriteLine($"Received response on queue '{context.Message.QueueName}'");
            ResetEvent.Set();
            return Task.CompletedTask;
        }
    }
}