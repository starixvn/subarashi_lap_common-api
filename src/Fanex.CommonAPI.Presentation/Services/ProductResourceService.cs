using System.Collections;
using System.Globalization;
using System.Resources;

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
            () => GetAllProductResources(language));
    }

    private Dictionary<string, string> GetAllProductResources(string language)
    {
        var culture = new CultureInfo(language);
        var result = new Dictionary<string, string>(_initialProductMap);
        var resourceSet = _resourceManager.GetResourceSet(culture, true, true);

        var productKeys = GetValidProductKeys(resourceSet);
        foreach (var key in productKeys)
        {
            var productCode = key.Replace("Product", string.Empty);
            if (result.ContainsKey(productCode))
            {
                continue;
            }

            var value = _resourceManager.GetString(key, culture);
            if (!string.IsNullOrEmpty(value))
            {
                result[productCode] = value;
            }
        }

        return result;
    }

    private static IEnumerable<string> GetValidProductKeys(ResourceSet resourceSet)
    {
        return resourceSet.Cast<DictionaryEntry>()
            .Select(entry => entry.Key.ToString())
            .Where(key => key.StartsWith("Product") && key != "Product");
    }
}