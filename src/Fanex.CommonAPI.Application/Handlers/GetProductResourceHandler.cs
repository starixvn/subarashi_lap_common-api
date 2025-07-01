using Azure.Core;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Helper;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Resources;
using Fanex.CommonAPI.Application.Services;
using Fanex.CommonAPI.Domain.Results;
using Fanex.Logging;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Globalization;
using System.Resources;
using VaultSharp.V1.SecretsEngines.TOTP;

namespace Fanex.CommonAPI.Application.Handlers;

public class GetProductResourceHandler : BaseHandler<GetProductResourceQuery, ServiceResult<Dictionary<string, string>>>
{
    private readonly ICacheProvider _cacheProvider;

    public GetProductResourceHandler(
        ICacheProvider cacheProvider,
        ILogger logger) : base(logger)
    {
        _cacheProvider = cacheProvider;
    }

    protected override async Task<ServiceResult<Dictionary<string, string>>> DoWork(GetProductResourceQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductResources(request);

        var result = _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            () => GetResult(request.Language, request.ProductCode),
            TimeSpan.FromDays(1));

        return ServiceResult<Dictionary<string, string>>.Success(result);
    }

    private Dictionary<string, string> GetResult(string language, string productCode)
    {
        var culture = new CultureInfo(language);

        if (!string.IsNullOrEmpty(productCode))
        {
            var value = ViewResources_Product.ResourceManager.GetString(
                GetResourceKeyByProductCode(productCode), culture) ?? string.Empty;

            return new Dictionary<string, string> { { productCode, value } };
        }

        var resourceSet = ViewResources_Product.ResourceManager.GetResourceSet(culture, true, true);
        var result = new Dictionary<string, string>();

        foreach (System.Collections.DictionaryEntry entry in resourceSet)
        {
            var key = entry.Key.ToString();
            var productKey = key.Replace("Product", string.Empty);

            if (ProductHelper.ProductNameMap.Any(x => x.Value == productKey))
            {
                productKey = ProductHelper.ProductNameMap.FirstOrDefault(x => x.Value == productKey).Key;
            }

            if (!result.ContainsKey(productKey))
            {
                var value = ViewResources_Product.ResourceManager.GetString(key, culture);

                if (!string.IsNullOrEmpty(value))
                {
                    result[productKey] = value;
                }
            }
        }

        return result;
    }

    private static string GetResourceKeyByProductCode(string productCode) =>
        ProductHelper.ProductNameMap.TryGetValue(productCode, out string keyValue) ? keyValue : $"Product{productCode}";
}