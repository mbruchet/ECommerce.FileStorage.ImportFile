using System;
using System.Text;
using System.Threading.Tasks;
using Import.Abstractions;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Import.Azure.Services
{
    public class AzureQueueReceiver<T> where T : class
    {
        #region " Public "

        public AzureQueueReceiver(AzureQueueSettings settings)
        {
            _settings = settings;
            Init();
        }

        public void Receive(
            Func<T, MessageProcessResponse> onProcess,
            Action<Exception> onError,
            Action onWait)
        {
            var options = new MessageHandlerOptions(e =>
            {
                onError(e.Exception);
                return Task.CompletedTask;
            })
            {
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(1)
            };

            _client.RegisterMessageHandler(
                async (message, token) =>
                {
                    try
                    {
                        // Get message
                        var data = Encoding.UTF8.GetString(message.Body);

                        T item;

                        if (typeof(T).Name.Equals("String", StringComparison.OrdinalIgnoreCase))
                            item = data as T;
                        else
                            item = JsonConvert.DeserializeObject<T>(data);

                        // Process message
                        var result = onProcess(item);

                        if (result == MessageProcessResponse.Complete)
                            await _client.CompleteAsync(message.SystemProperties.LockToken);
                        else if (result == MessageProcessResponse.Abandon)
                            await _client.AbandonAsync(message.SystemProperties.LockToken);
                        else if (result == MessageProcessResponse.Dead)
                            await _client.DeadLetterAsync(message.SystemProperties.LockToken);

                        // Wait for next message
                        onWait();
                    }
                    catch (Exception ex)
                    {
                        await _client.DeadLetterAsync(message.SystemProperties.LockToken);
                        onError(ex);
                    }
                }, options);
        }

        #endregion

        #region " Private "

        private readonly AzureQueueSettings _settings;
        private QueueClient _client;

        private void Init()
        {
            _client = new QueueClient(_settings.ConnectionString, _settings.QueueName);
        }

        #endregion
    }
}
