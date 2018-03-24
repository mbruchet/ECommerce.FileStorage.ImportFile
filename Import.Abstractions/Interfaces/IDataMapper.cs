using System.Collections.Generic;
using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface IDataMapper<T>
    {
        IEnumerable<T> MapData(string json);
    }
}
