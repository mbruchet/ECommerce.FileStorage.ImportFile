using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface IImportService
    {
        Task<bool> DownloadFile(string fileName, bool headerOnFirstRow, string separator);
    }
}