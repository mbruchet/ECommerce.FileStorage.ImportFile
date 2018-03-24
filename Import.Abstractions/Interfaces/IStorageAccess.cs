using System.IO;
using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface IStorageAccess
    {
        Task<MemoryStream> DownloadAsync(string blobName);
        Task DownloadAsync(string blobName, string path);
        Task RemoveFileAsync(string fileName);
    }
}