using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Import.Abstractions;
using Import.Abstractions.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;

namespace Import.Azure
{
    public class AzureStorageAccess : IStorageAccess
    {
        private readonly AzureBlobSettings _settings;

        public AzureStorageAccess(IOptions<AzureBlobSettings> settings)
        {
            _settings = settings.Value;
        }

        public AzureStorageAccess(AzureBlobSettings settings)
        {
            _settings = settings;
        }

        private async Task<CloudFileShare> GetFileShareAsync()
        {
            //Account
            var storageAccount = new CloudStorageAccount(new StorageCredentials(_settings.StorageAccount, 
                _settings.StorageKey), _settings.UseHttps);

            //Client
            var fileClient = storageAccount.CreateCloudFileClient();

            //Container
            var cloudFileShare = fileClient.GetShareReference(_settings.ShareName);
            await cloudFileShare.CreateIfNotExistsAsync();

            return cloudFileShare;
        }

        private async Task<CloudFile> GetFileAsync(string fileName)
        {
            //Container
            var fileShare = await GetFileShareAsync();

            //Blob
            var rootFolder = fileShare.GetRootDirectoryReference();
            var cloudFile = rootFolder.GetFileReference(fileName);

            return await cloudFile.ExistsAsync() ? cloudFile : null;
        }


        public async Task<MemoryStream> DownloadAsync(string blobName)
        {
            //Reference
            var file = await GetFileAsync(blobName);

            //Download
            var stream = new MemoryStream();
            await file.DownloadToStreamAsync(stream);

            return stream;
        }

        public async Task DownloadAsync(string blobName, string path)
        {
            //Reference
            var file = await GetFileAsync(blobName);

            //Download
            await file.DownloadToFileAsync(path, FileMode.Create);
        }

        public async Task RemoveFileAsync(string fileName)
        {
            //Container
            var fileShare = await GetFileShareAsync();

            //Blob
            var rootFolder = fileShare.GetRootDirectoryReference();
            var cloudFile = rootFolder.GetFileReference(fileName);

            if(await cloudFile.ExistsAsync())
                await cloudFile.DeleteAsync();
        }

        public async Task UploadAsync(string filePath, string folder, string fileName)
        {
            var fileShare = await GetFileShareAsync();
            var rootFolder = fileShare.GetRootDirectoryReference();

            var targetDirectory =  rootFolder.GetDirectoryReference(folder);

            if (!await targetDirectory.ExistsAsync() && !await targetDirectory.CreateIfNotExistsAsync())
                    throw new InvalidOperationException($"can not create folder target {folder}");

            var targetFile = targetDirectory.GetFileReference(fileName);

            if (!await targetFile.ExistsAsync() || await targetFile.DeleteIfExistsAsync())
                await targetFile.UploadFromFileAsync(filePath);
        }
        public async Task UploadAsync(Stream fileStream, string folder, string fileName)
        {
            var fileShare = await GetFileShareAsync();
            var rootFolder = fileShare.GetRootDirectoryReference();

            var targetDirectory = rootFolder.GetDirectoryReference(folder);

            if (!await targetDirectory.CreateIfNotExistsAsync())
                return;

            var targetFile = targetDirectory.GetFileReference(fileName);

            if (await targetFile.DeleteIfExistsAsync())
                await targetFile.UploadFromStreamAsync(fileStream);
        }

        public async Task UploadAsync(byte[] fileContent, int index, int count, string folder, string fileName)
        {
            var fileShare = await GetFileShareAsync();
            var rootFolder = fileShare.GetRootDirectoryReference();

            var targetDirectory = rootFolder.GetDirectoryReference(folder);

            if (!await targetDirectory.CreateIfNotExistsAsync())
                return;

            var targetFile = targetDirectory.GetFileReference(fileName);

            if (await targetFile.DeleteIfExistsAsync())
                await targetFile.UploadFromByteArrayAsync(fileContent, index, count);
        }

    }
}
