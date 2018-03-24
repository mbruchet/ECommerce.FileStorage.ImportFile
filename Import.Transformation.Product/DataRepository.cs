using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ecommerce.Data.RepositoryStore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Import.Mapping.Product
{
    public class DataRepository
    {
        public RepositoryStoreFactory<ProductModel> ProductRepository { get; }
        public RepositoryStoreFactory<ProductModel> ModelRepository { get; }
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ILogger<DataRepository> _logger;

        public DataRepository(RepositoryStoreFactory<ProductModel> productRepository,
            RepositoryStoreFactory<ProductModel> modelRepository, DiagnosticSource diagnosticSource,
            ILogger<DataRepository> logger)
        {
            ProductRepository = productRepository;
            ModelRepository = modelRepository;
            _diagnosticSource = diagnosticSource;
            _logger = logger;
        }

        public void ImportData(IEnumerable<ProductModel> data)
        {
            var activity = new Activity(nameof(ImportData));

            if (_diagnosticSource.IsEnabled(activity.OperationName))
                _diagnosticSource.StartActivity(activity, 1);

            var countProducts = 0;
            var countModels = 0;
            var productId = 0;
            var modelId = 0;
            var lineIndex = 0;

            foreach (var item in data)
            {
                lineIndex++;

                try
                {
                    item._id = ObjectId.GenerateNewId();

                    if (item.RecordType.Equals("PRODUCT", StringComparison.OrdinalIgnoreCase))
                    {
                        productId++;
                        item.Id = productId;
                        ProductRepository.AddAsync(item).Wait();
                        countProducts++;
                    }
                    else
                    {
                        modelId++;
                        item.Id = modelId;
                        ModelRepository.AddAsync(item).Wait();
                        countModels++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"impossible d'importer la ligne {lineIndex}");
                }
            }

            _diagnosticSource.StopActivity(activity, new
            {
                Products=countProducts, Models = countModels
            });
        }
    }
}
