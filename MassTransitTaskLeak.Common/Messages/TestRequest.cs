namespace MassTransitTaskLeak.Common.Messages
{
    public class TestRequest
    {
        public TestRequest(string responseQueue)
        {
            ResponseQueue = responseQueue;
        }

        public string ResponseQueue { get; }
    }
}