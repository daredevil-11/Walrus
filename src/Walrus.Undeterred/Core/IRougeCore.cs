using System.Threading.Tasks;

namespace Walrus.Undeterred.Core
{
    public interface IRougeCore
    {
        Task<bool> Test();
        Task<bool> Get();
    }
}
