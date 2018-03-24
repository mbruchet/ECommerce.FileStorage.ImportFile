using System.IO;
using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface IStorageAccess
    {
        Task<MemoryStream> DownloadAsync(string blobName);
        Task DownloadAsync(string blobName, string path);
        Task RemoveFileAsync(string fileName);
        Task UploadAsync(string filePath, string folder, string fileName);
        Task UploadAsync(Stream fileStream, string folder, string fileName);
        Task UploadAsync(byte[] fileContent, int index, int count, string folder, string fileName);
    }
}