using Import.Abstractions;
using Import.Abstractions.Interfaces;
using Import.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Import.Azure
{
    public static class AzureExtension
    {
        public static void AddAzureImport(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureBlobSettings>(configuration);
            services.Configure<AzureQueueSettings>(configuration);
            services.Configure<TransformationSettings>(configuration);

            services.AddSingleton<IStorageAccess, AzureStorageAccess>();
            services.AddSingleton<IQueueService, AzureQueueService>();

        }
    }
}
