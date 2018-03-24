using System.Threading.Tasks;
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

        public async Task<bool> Enqueue(string xmlContent)
        {
            var sender = new AzureQueueSender<string>(_settings);
            await sender.SendAsync(xmlContent);
            return true;
        }
    }
}
