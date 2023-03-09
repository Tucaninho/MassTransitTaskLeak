namespace MassTransitTaskLeak.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransitTaskLeak.Common;
    using MassTransitTaskLeak.Common.Consumers;
    using MassTransitTaskLeak.Common.Messages;

    public static class Program
    {
        public static async Task Main()
        {
            string queueName = Helper.GetQueueName();
            var helper = new Helper();
            await using (helper.ConfigureAwait(false))
            {
                await helper.CreateMessageBusAsync().ConfigureAwait(false);

                using (var cts = new CancellationTokenSource())
                {
                    if (Helper.ChangeQueueEveryRequest)
                    {
                        Console.WriteLine("Queue name change for every request, Server tasks should grow");
                    }
                    else
                    {
                        Console.WriteLine("Queue name will always be the same, Server tasks should remain constant");
                    }

                    Console.WriteLine("### Put a breakpoint in Server ConsumerRequest to check the number of tasks ###");
                    Console.WriteLine("Press a key to start tests");
                    Console.ReadKey();

                    var task = Task.Run(
                        async () =>
                            {
                                while (!cts.IsCancellationRequested)
                                {
                                    if (Helper.ChangeQueueEveryRequest)
                                    {
                                        queueName = Helper.GetQueueName();
                                    }

                                    await helper.AddQueueAsync(queueName, typeof(ConsumerResponse), _ => new ConsumerResponse()).ConfigureAwait(false);

                                    await helper.SendToAsync(Helper.StaticQueueName, new TestRequest(queueName)).ConfigureAwait(false);

                                    var responseReceived = ConsumerResponse.ResetEvent.WaitOne(TimeSpan.FromMinutes(10));
                                    Console.WriteLine($"Response received on queue '{queueName}': {responseReceived}");

                                    await helper.StopQueueAsync(queueName).ConfigureAwait(false);
                                }
                            },
                        cts.Token);

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                    cts.Cancel();

                    await task.ConfigureAwait(false);
                }

            }
        }
    }
}