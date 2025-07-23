namespace Fanex.CommonAPI.Infrastructure.Services;

using Application.Services;
using Microsoft.Extensions.Caching.Memory;

public class CacheProvider(IMemoryCache cache) : ICacheProvider
{
    public async Task<T> GetOrCreateFromCacheAsync<T>(string key, Func<Task<T>> action, TimeSpan? cacheTime)
    {
        if (!cache.TryGetValue(key, out T data))
        {
            data = await action();
            if (data is not null)
            {
                SetValueToCache(key, data, cacheTime);
            }
        }

        return data;
    }

    public T GetOrCreateFromCache<T>(string key, Func<T> action, TimeSpan? cacheTime)
    {
        if (!cache.TryGetValue(key, out T data))
        {
            data = action();
            if (data is not null)
            {
                SetValueToCache(key, data, cacheTime);
            }
        }

        return data;
    }

    public T GetOrCreateFromCache<T>(string key, T value, TimeSpan? cacheTime)
    {
        if (!cache.TryGetValue(key, out T data) && data is not null)
        {
            SetValueToCache(key, value, cacheTime);
        }

        return data;
    }

    public T GetValueFromCache<T>(string key, T defaultValue = default)
    {
        return cache.TryGetValue(key, out T data) ? data : defaultValue;
    }

    public void RemoveFromCache(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key), "Key should not be empty!");
        }

        cache.Remove(key);
    }

    private void SetValueToCache<T>(string key, T value, TimeSpan? cacheTime)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key), "Key should not be empty!");
        }

        if (Equals(value, default(T)))
        {
            return;
        }

        const int defaultExpiredMinutes = 15;
        cache.Set(key, value, cacheTime ?? TimeSpan.FromMinutes(defaultExpiredMinutes));
    }
}