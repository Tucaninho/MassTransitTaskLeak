namespace MassTransitTaskLeak.Common
{

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MassTransit;

    public class Helper : IAsyncDisposable
    {
        // if set to true,Server leaks tasks, if set to false server tasks are constant
        public const bool ChangeQueueEveryRequest = true;

        public const string QueueNamePrefix = "MassTransitTaskLeak-";

        public static string StaticQueueName;

        private readonly Uri HostAddress;

        private readonly string Password;

        private readonly string Username;

        private IBusControl _busControl;

        private Dictionary<string, HostReceiveEndpointHandle> _endpoints;

        private bool _isDisposed;

        static Helper()
        {
            StaticQueueName = GetQueueName();
        }

        public Helper()
        {
            _isDisposed = false;
            _endpoints = new Dictionary<string, HostReceiveEndpointHandle>();

            HostAddress = new Uri("rabbitmq://localhost/");
            Username = "guest";
            Password = "guest";
        }

        public static string GetQueueName()
        {
            return QueueNamePrefix + Guid.NewGuid();
        }

        public async ValueTask DisposeAsync()
        {
            lock (_endpoints)
            {
                _isDisposed = true;
            }

            foreach (KeyValuePair<string, HostReceiveEndpointHandle> ep in _endpoints)
            {
                await ep.Value.StopAsync().ConfigureAwait(false);
            }

            if (_busControl != null)
            {
                await _busControl.StopAsync().ConfigureAwait(false);
            }
        }

        public Task CreateMessageBusAsync()
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(
                x =>
                    {
                        x.Host(
                            HostAddress,
                            cfg =>
                                {
                                    cfg.Username(Username);
                                    cfg.Password(Password);
                                });
                    });

            return _busControl.StartAsync();
        }

        public Task AddQueueAsync(string queueName, Type consumerType, Func<Type, object> consumerFactory)
        {
            lock (_endpoints)
            {
                if (_isDisposed)
                {
                    throw new Exception("Already disposed");
                }

                if (_endpoints.ContainsKey(queueName))
                {
                    throw new ArgumentException($"Queue '{queueName}' already present", nameof(queueName));
                }

                HostReceiveEndpointHandle endpoint = _busControl.ConnectReceiveEndpoint(
                    queueName,
                    cfg =>
                        {
                            var rmqCfg = (IRabbitMqReceiveEndpointConfigurator)cfg;
                            rmqCfg.AutoDelete = true;
                            rmqCfg.Durable = false;
                            rmqCfg.PurgeOnStartup = true;

                            cfg.Consumer(consumerType, consumerFactory);
                        });

                _endpoints[queueName] = endpoint;

                Console.WriteLine($"Adding queue '{queueName}' for consumer '{consumerType.Name}'");

                return endpoint.Ready;
            }
        }

        public Task StopQueueAsync(string queueName)
        {
            lock (_endpoints)
            {
                if (_isDisposed)
                {
                    throw new Exception("Already disposed");
                }

                if (!_endpoints.Remove(queueName, out var endpoint))
                {
                    throw new ArgumentException($"Queue '{queueName}' not found", nameof(queueName));
                }

                Console.WriteLine($"Removing queue '{queueName}");
                return endpoint.StopAsync();
            }
        }

        public async Task SendToAsync<T>(string queueName, T message)
            where T : class
        {
            var uri = HostAddress + queueName + "?temporary=true";
            var endpoint = await _busControl.GetSendEndpoint(new Uri(uri)).ConfigureAwait(false);

            Console.WriteLine($"Sending '{typeof(T).Name}' to '{uri}'");
            await endpoint.Send(message).ConfigureAwait(false);
        }
    }
}