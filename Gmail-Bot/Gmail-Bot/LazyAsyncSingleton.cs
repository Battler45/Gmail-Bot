using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutomatedEmailChecker
{
    public class LazyAsyncSingleton<T>: IDisposable
        where T : class, IDisposable
    {
        private IAsyncFactory<T> Factory { get; set; }
        private T Instance { get; set; }
        private readonly object _locker = new object();

        public LazyAsyncSingleton(IAsyncFactory<T> factory)
        {
            Factory = factory;
        }

        public async Task<T> GetInstanceAsync()
        {
            if (Instance != null) return Instance;
            
            Monitor.Enter(_locker);
            Instance ??= await Factory.CreateAsync();
            Monitor.Exit(_locker);

            return Instance;
        }
        public void Dispose()
        {
            Monitor.Enter(_locker);
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
            Factory = null;
            Monitor.Exit(_locker);
        }
    }
}