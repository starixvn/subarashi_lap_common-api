using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Helper;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Resources;
using Fanex.CommonAPI.Application.Services;
using Fanex.CommonAPI.Domain.Results;
using Fanex.Logging;
using System.Globalization;

namespace Fanex.CommonAPI.Application.Handlers;

public class GetProductResourceHandler : BaseHandler<GetProductResourceQuery, ServiceResult<object>>
{
    private readonly ICacheProvider _cacheProvider;

    public GetProductResourceHandler(
        ICacheProvider cacheProvider,
        ILogger logger) : base(logger)
    {
        _cacheProvider = cacheProvider;
    }

    protected override async Task<ServiceResult<object>> DoWork(GetProductResourceQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductResources(request);

        var result = _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            () => GetResult(request.Language, request.ProductCode),
            TimeSpan.FromDays(1));

        return ServiceResult<object>.Success(result);
    }

    private static object GetResult(string language, string productCode)
    {
        var culture = new CultureInfo(language);

        if (!string.IsNullOrEmpty(productCode))
        {
            var resourceKey = GetResourceKeyByProductCode(productCode);
            var value = ViewResources_Product.ResourceManager.GetString(resourceKey, culture)
                       ?? ViewResources_Product.ResourceManager.GetString(resourceKey)
                       ?? string.Empty;

            return value;
        }

        var cultureResourceSet = ViewResources_Product.ResourceManager.GetResourceSet(culture, true, true);
        var defaultResourceSet = ViewResources_Product.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
        var result = new Dictionary<string, string>();

        PopulateResourceDictionary(defaultResourceSet, result);
        PopulateResourceDictionary(cultureResourceSet, result);

        return result;
    }

    private static void PopulateResourceDictionary(System.Resources.ResourceSet resourceSet, Dictionary<string, string> result)
    {
        foreach (System.Collections.DictionaryEntry entry in resourceSet)
        {
            var key = entry.Key.ToString();
            var productKey = key.Replace("Product", string.Empty);

            var existingEntry = ProductHelper.ProductNameMap.FirstOrDefault(x => x.Value == productKey);
            if (!existingEntry.Equals(default(KeyValuePair<string, string>)))
            {
                productKey = existingEntry.Key;
            }

            var value = entry.Value?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                result[productKey] = value;
            }
        }
    }

    private static string GetResourceKeyByProductCode(string productCode) =>
        ProductHelper.ProductNameMap.TryGetValue(productCode, out string keyValue) ? keyValue : $"Product{productCode}";
}