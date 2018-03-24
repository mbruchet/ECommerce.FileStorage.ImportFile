using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ecommerce.Data.RepositoryStore;
using Import.Abstractions;
using Import.Abstractions.Interfaces;
using Import.Azure;
using Import.Azure.Services;
using Import.Mapping.Product;
using Import.Transformation.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DataMapper = Import.Mapping.Product.DataMapper;
using DataRepository = Import.Mapping.Product.DataRepository;

namespace Import.Transformation
{
    class Program
    {
        private static IQueueService _inputQueueService;
        private static IQueueService _outputQueueService;
        private static AzureQueueSettings _outputQueueSettings;

        private static DataMapper _dataMapper;
        private static DataRepository _dataRepo;

        static void Main(string[] args)
        {
            PreloadAllAssemblies();
            Init();

            Console.WriteLine("Attente d'un message dans la file d'attente");

            _inputQueueService.Receive<string>((message) =>
            {
                Console.WriteLine("un message a été reçu, démarrage du traitement");

                //1°) Conversion vers Json
                var json = new XmlToJsonConverter().ConvertToJson(message);

                //2°) Mapping Produit
                var data = _dataMapper.MapData(json);

                //3°) Import dans le repository
                _dataRepo.ImportData(data);

                //5°) Mettre en file d'attente le message
                _outputQueueService.Enqueue(json).Wait();

                return MessageProcessResponse.Complete;
            }, (ex) =>
            {
                Console.WriteLine($"Attention une erreur a été rencontrée : {ex}");
            }, () => { Task.Delay(100).Wait(); });

            Console.WriteLine("press key to exit");
            Console.ReadKey();
        }

        private static void PreloadAllAssemblies()
        {
            var binDirectory = new DirectoryInfo("bin").FullName;

            var files = Directory.Exists(binDirectory) ? new DirectoryInfo(binDirectory).GetFiles("*.dll") :
                new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.dll");

            foreach (var file in files)
            {
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => !x.IsDynamic && x.Location == file.FullName))
                    Assembly.Load(Path.GetFileNameWithoutExtension(file.FullName));
            }
        }

        private static void Init()
        {
            //Configuration initialisation des différents objets
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true, false);

            var configuration = configurationBuilder.Build();

            //initialisation du système de log
            var loggerFactory = new LoggerFactory().AddConsole(configuration);

            //initialisation des files d'attente
            var settings = new AzureQueueSettings();
            configuration.GetSection("InputQueue").Bind(settings);

            _inputQueueService = new AzureQueueService(settings);

            _outputQueueSettings = new AzureQueueSettings();
            configuration.GetSection("OutputQueue").Bind(_outputQueueSettings);            

            _outputQueueService = new AzureQueueService(_outputQueueSettings);

            //initialisation du repository
            _dataMapper = new DataMapper();

            var productRepositorySettings = new RepositorySettings();
            configuration.GetSection("Repositories:Product").Bind(productRepositorySettings);

            var modelRepositorySettings = new RepositorySettings();
            configuration.GetSection("Repositories:Model").Bind(modelRepositorySettings);

            var diagnosticSource = new MyDiagnosticSource();

            var productRepository = new RepositoryStoreFactory<ProductModel>(productRepositorySettings.Provider, new ConnectionOptions
            {
                Provider = productRepositorySettings.ProviderType,
                ConnectionString = JsonConvert.SerializeObject(productRepositorySettings.ConnectionString)
            }, loggerFactory, diagnosticSource);

            var modelRepository = new RepositoryStoreFactory<ProductModel>(modelRepositorySettings.Provider, new ConnectionOptions
            {
                Provider = productRepositorySettings.ProviderType,
                ConnectionString = JsonConvert.SerializeObject(modelRepositorySettings.ConnectionString)
            }, loggerFactory, diagnosticSource);

            _dataRepo = new DataRepository(productRepository, modelRepository, diagnosticSource, loggerFactory.CreateLogger<DataRepository>());

            //initialisation du partage Azure

            var azureBlobSettings = new AzureBlobSettings();
            configuration.Bind(azureBlobSettings);

            new AzureStorageAccess(azureBlobSettings);
        }
    }
}
