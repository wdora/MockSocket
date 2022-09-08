namespace MockSocket.Cache
{
    public interface ICacheService
    {
        void Add<T>(string key, T value);

        T Get<T>(string key);

        void Remove(string key);
    }
}
