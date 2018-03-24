using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface IQueueService
    {
        Task<bool> Enqueue(string xmlContent);
    }
}
