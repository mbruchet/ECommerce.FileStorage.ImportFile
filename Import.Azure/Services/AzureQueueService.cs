using System;
using System.Threading.Tasks;
using Import.Abstractions;
using Import.Abstractions.Interfaces;
using Microsoft.Extensions.Options;

namespace Import.Azure.Services
{
    public class AzureQueueService:IQueueService
    {
        private readonly AzureQueueSettings _settings;

        public AzureQueueService(IOptions<AzureQueueSettings> settingsOptions)
        {
            _settings = settingsOptions.Value;
        }

        public AzureQueueService(AzureQueueSettings settings)
        {
            _settings = settings;
        }

        public async Task<bool> Enqueue<T>(T content) where T:class 
        {
            var sender = new AzureQueueSender<T>(_settings);
            await sender.SendAsync(content);
            return true;
        }

        public void Receive<T>(Func<T, MessageProcessResponse> onProcess,
            Action<Exception> onError,
            Action onWait) where T : class
        {
            var receiver = new AzureQueueReceiver<T>(_settings);
            receiver.Receive(onProcess, onError, onWait);
        }
    }
}
