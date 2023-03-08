namespace MassTransitTaskLeak.Common.Messages
{

    public class TestResponse
    {
        public TestResponse(string queueName)
        {
            QueueName = queueName;
        }

        public string QueueName { get; }
    }
}