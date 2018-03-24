using System;
using System.IO;
using System.Threading.Tasks;
using Import.Abstractions;
using Import.Abstractions.Interfaces;
using Microsoft.Extensions.Options;

namespace Import.Service
{
    public class ImportService : IImportService
    {
        private readonly IStorageAccess _storageAccess;
        private readonly IQueueService _queueService;
        private readonly ITransformationFileService _transformationFileService;
        private readonly TransformationSettings _transformationSettings;

        public ImportService(IStorageAccess storageAccess, IQueueService queueService, ITransformationFileService transformationFileService, IOptions<TransformationSettings> transformationSettings)
        {
            _storageAccess = storageAccess;
            _queueService = queueService;
            _transformationFileService = transformationFileService;
            _transformationSettings = transformationSettings.Value;
        }

        public async Task<bool> DownloadFile(string fileName, bool headerOnFirstRow, string separator)
        {
            if(string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));

            if (!Directory.Exists(_transformationSettings.TargetFolder))
                Directory.CreateDirectory(_transformationSettings.TargetFolder);

            var targetFile = Path.Combine(_transformationSettings.TargetFolder, Path.GetFileName(fileName));

            if(File.Exists(targetFile))
                File.Move(targetFile, Path.ChangeExtension(targetFile, $".{DateTime.Now:yyyyMMddHHHmmss}.bak"));

            await _storageAccess.DownloadAsync(fileName, targetFile);

            if (!File.Exists(targetFile)) return false;

            var xDoc = await _transformationFileService.TransformFile(targetFile, new TransformationSettings
            {
                TargetFolder = targetFile, HeaderOnFirstRow = headerOnFirstRow, Separator = separator
            });

            var isSuccess = !string.IsNullOrEmpty(xDoc) && await _queueService.Enqueue(xDoc);

            if (isSuccess)
                await _storageAccess.RemoveFileAsync(fileName);

            return isSuccess;
        }
    }
}
