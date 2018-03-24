using Import.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Import.Service
{
    public static class ImportServiceExtension
    {
        public static void AddImportService(this IServiceCollection services)
        {
            services.AddSingleton<IImportService, ImportService>();
        }
    }
}
