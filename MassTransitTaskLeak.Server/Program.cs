namespace MassTransitTaskLeak.Server
{
    using System;
    using System.Threading.Tasks;

    using MassTransitTaskLeak.Common;
    using MassTransitTaskLeak.Common.Consumers;

    public static class Program
    {
        public static async Task Main()
        {
            var helper = new Helper();
            await using (helper.ConfigureAwait(false))
            {
                await helper.CreateMessageBusAsync().ConfigureAwait(false);

                await helper.AddQueueAsync(Helper.StaticQueueName, typeof(ConsumerRequest), _ => new ConsumerRequest(helper)).ConfigureAwait(false);

                Console.WriteLine("Press a key to exit");
                Console.ReadKey();

                await helper.StopQueueAsync(Helper.StaticQueueName).ConfigureAwait(false);
            }
        }
    }
}