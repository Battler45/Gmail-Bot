using System.Threading.Tasks;

namespace AutomatedEmailChecker
{
    public interface IAsyncFactory<T>
    {
        Task<T> CreateAsync();
    }
}