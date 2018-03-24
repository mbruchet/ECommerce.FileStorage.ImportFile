using System;
using System.IO;
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
        private static AzureStorageAccess _azureStorageAccess;
        private static string _productRepositoryFile;
        private static string _modelRepositoryFile;

        static void Main(string[] args)
        {
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

                //4°) Partage des fichiers
                var productFileName = Path.GetFileName(_productRepositoryFile);
                var modelFileName = Path.GetFileName(_modelRepositoryFile);

                _azureStorageAccess.UploadAsync(_productRepositoryFile, "traitement", productFileName).Wait();
                _azureStorageAccess.UploadAsync(_modelRepositoryFile, "traitement", modelFileName).Wait();

                //5°) Mettre en file d'attente le message
                _outputQueueService.Enqueue(JsonConvert.SerializeObject(new
                {
                    ProductFileName = productFileName,
                    ModelFileName = modelFileName
                })).Wait();

                return MessageProcessResponse.Complete;
            }, (ex) =>
            {
                Console.WriteLine($"Attention une erreur a été rencontrée : {ex}");
            }, () => { Task.Delay(100).Wait(); });

            Console.WriteLine("press key to exit");
            Console.ReadKey();
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
            var repositorySettings = new RepositorySettings();
            configuration.GetSection("Repository").Bind(repositorySettings);

            var diagnosticSource = new MyDiagnosticSource();

            _productRepositoryFile = repositorySettings.ConnectionString + "\\Products.json";
            _modelRepositoryFile = repositorySettings.ConnectionString + "\\models.json";

            var productRepository = new RepositoryStoreFactory<ProductModel>(repositorySettings.Provider, new ConnectionOptions
            {
                Provider = repositorySettings.ProviderType,
                ConnectionString = _productRepositoryFile
            }, loggerFactory, diagnosticSource);

            var modelRepository = new RepositoryStoreFactory<ProductModel>(repositorySettings.Provider, new ConnectionOptions
            {
                Provider = repositorySettings.ProviderType,
                ConnectionString = _modelRepositoryFile
            }, loggerFactory, diagnosticSource);

            _dataRepo = new DataRepository(productRepository, modelRepository, diagnosticSource, loggerFactory.CreateLogger<DataRepository>());

            //initialisation du partage Azure

            var azureBlobSettings = new AzureBlobSettings();
            configuration.Bind(azureBlobSettings);

            _azureStorageAccess = new AzureStorageAccess(azureBlobSettings);
        }
    }
}
