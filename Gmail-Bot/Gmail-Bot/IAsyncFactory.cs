using System.Threading.Tasks;

namespace GmailBot
{
    public interface IAsyncFactory<T>
    {
        Task<T> CreateAsync();
    }
}