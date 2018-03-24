using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Import.Azure.Services
{
    internal class AzureQueueSender<T> where T : class
    {
        #region " Public "

        public AzureQueueSender(AzureQueueSettings settings)
        {
            _settings = settings;
            Init();
        }

        public async Task SendAsync(T item)
        {
            await SendAsync(item, null);
        }

        public async Task SendAsync(T item, Dictionary<string, object> properties)
        {
            var json = JsonConvert.SerializeObject(item);
            var message = new Message(Encoding.UTF8.GetBytes(json));

            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    message.UserProperties.Add(prop.Key, prop.Value);
                }
            }

            await _client.SendAsync(message);
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
