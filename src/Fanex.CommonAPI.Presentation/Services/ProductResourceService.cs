using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Fanex.CommonAPI.Application.Services;

namespace Fanex.CommonAPI.Presentation.Services;

public class ProductResourceService : IProductResourceService
{
    private readonly ICacheService _cacheService;
    private readonly ResourceManager _resourceManager;
    private readonly Dictionary<string, string> _initialProductMap;

    public ProductResourceService(
        ICacheService cacheService,
        ResourceManager resourceManager,
        Dictionary<string, string> initialProductMap)
    {
        _cacheService = cacheService;
        _resourceManager = resourceManager;
        _initialProductMap = initialProductMap;
    }

    public Dictionary<string, string> GetProductResources(string language)
    {
        var cacheKey = $"ProductResources_{language}";

        return _cacheService.GetOrCreateFromCache(
            cacheKey,
            () => GetAllProductResources(language),
            null);
    }

    private Dictionary<string, string> GetAllProductResources(string language)
    {
        var culture = new CultureInfo(language);
        var result = new Dictionary<string, string>(_initialProductMap);

        var resourceSet = _resourceManager.GetResourceSet(culture, true, true);

        foreach (System.Collections.DictionaryEntry entry in resourceSet)
        {
            var key = entry.Key.ToString();
            if (key.StartsWith("Product") && key != "Product")
            {
                var productCode = key.Replace("Product", string.Empty);
                if (!result.ContainsKey(productCode))
                {
                    var value = _resourceManager.GetString(key, culture);
                    if (!string.IsNullOrEmpty(value))
                    {
                        result[productCode] = value;
                    }
                }
            }
        }

        return result;
    }
}