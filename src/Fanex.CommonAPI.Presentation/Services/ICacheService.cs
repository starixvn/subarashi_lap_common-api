using System;

namespace Fanex.CommonAPI.Presentation.Services;

public interface ICacheService
{
    T GetOrCreateFromCache<T>(string key, Func<T> factory, TimeSpan? cacheTime = null);
}