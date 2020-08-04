using System;
using System.Threading;
using System.Threading.Tasks;

namespace GmailBot
{
    public class LazyAsyncSingleton<T>: IDisposable
        where T : class, IDisposable
    {
        private IAsyncFactory<T> Factory { get; set; }
        private T Instance { get; set; }
        private readonly object locker = new object();

        public LazyAsyncSingleton(IAsyncFactory<T> factory)
        {
            Factory = factory;
        }

        public async Task<T> GetInstanceAsync()
        {
            if (Instance != null) return Instance;
            
            Monitor.Enter(locker);
            Instance ??= await Factory.CreateAsync();
            Monitor.Exit(locker);

            return Instance;
        }
        public void Dispose()
        {
            Monitor.Enter(locker);
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
            Factory = null;
            Monitor.Exit(locker);
        }
    }
}