using System;
using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface IQueueService
    {
        Task<bool> Enqueue<T>(T content) where T : class;

        void Receive<T>(Func<T, MessageProcessResponse> onProcess,
            Action<Exception> onError,
            Action onWait) where T : class;
    }
}
