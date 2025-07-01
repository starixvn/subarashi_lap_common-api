using System;
using Fanex.CommonAPI.Application.Services;

namespace Fanex.CommonAPI.Presentation.Services;

public class CacheService : ICacheService
{
    private readonly ICacheProvider _cacheProvider;

    public CacheService(ICacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    public T GetOrCreateFromCache<T>(string key, Func<T> factory, TimeSpan? cacheTime = null)
    {
        return _cacheProvider.GetOrCreateFromCache(key, factory, cacheTime);
    }
}