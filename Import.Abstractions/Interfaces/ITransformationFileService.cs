using System.Threading.Tasks;

namespace Import.Abstractions.Interfaces
{
    public interface ITransformationFileService
    {
        Task<string> TransformFile(string fileName, TransformationSettings settings);
    }
}
