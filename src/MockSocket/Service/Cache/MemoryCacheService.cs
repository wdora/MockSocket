using System.Collections.Concurrent;

namespace MockSocket.Cache
{
    public class InMemoryCacheService : ICacheService
    {
        ConcurrentDictionary<string, object> cache = new();

        public void Add<T>(string key, T value)
        {
            cache.AddOrUpdate(key, value!, (k, oldV) => value!);
        }

        public void Remove(string key)
        {
            cache.TryRemove(key, out _);
        }

        public T Get<T>(string key)
        {
            return cache.TryGetValue(key, out var val) ? (T)val : default!;
        }
    }
}
