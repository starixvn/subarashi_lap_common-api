namespace Fanex.CommonAPI.Application.Handlers;

using AsyncKeyedLock;
using Domain.Entities;
using Domain.Params;
using Domain.Repositories;
using Domain.Results;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Helper;
using Fanex.CommonAPI.Application.Services;
using Fanex.Logging;
using Microsoft.AspNetCore.Hosting;
using Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

public class GetProductAccessHandler(
    ILogger logger,
    IProductRepository productRepository,
    ICacheProvider cacheProvider,
    IWebHostEnvironment webHostEnvironment)
    : BaseHandler<GetProductAccessQuery, ServiceResult<IEnumerable<Product>>>(logger)
{
    private const int Timeout = 500;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private static readonly AsyncKeyedLocker<string> AsyncKeyedLocker = new(new AsyncKeyedLockOptions
    {
        MaxCount = 1, // max request call at the same time
        PoolSize = 0 // disable pooling
    });

    protected override async Task<ServiceResult<IEnumerable<Product>>> DoWork(
        GetProductAccessQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductAccess(request);

        var param = new GetProductAccessParam
        {
            LicenseeId = request.LicenseeId,
            FunctionKey = request.FunctionKey,
            Language = request.Language
        };

        var result = await GetResult(param, cacheKey);

        return result;
    }

    private async Task<ServiceResult<IEnumerable<Product>>> GetResult(GetProductAccessParam param, string cacheKey)
    {
        var products = cacheProvider.GetValueFromCache<IEnumerable<Product>>(cacheKey);

        if (products is not null)
        {
            return ServiceResult<IEnumerable<Product>>.Success(products);
        }

        using (await AsyncKeyedLocker.LockOrNullAsync(cacheKey, Timeout))
        {
            products = await cacheProvider
                .GetOrCreateFromCacheAsync(
                    cacheKey,
                    async () => await GetProductList(param).ConfigureAwait(false),
                    TimeSpan.FromMinutes(15));

            return ServiceResult<IEnumerable<Product>>.Success(products);
        }
    }

    private async Task<IEnumerable<Product>> GetProductList(GetProductAccessParam param)
    {
        var allowedProducts = GetAllowedProducts(param.FunctionKey);
        var originalProducts = await productRepository.GetProductAccesssAsync(param.LicenseeId).ConfigureAwait(false);
        var culture = new System.Globalization.CultureInfo(string.IsNullOrEmpty(param.Language) ? "en-US" : param.Language);

        var productList = allowedProducts.Any() ? originalProducts.Where(i => allowedProducts.Contains(i.ProductCode)).ToList()
                : originalProducts;

        foreach (var item in productList)
        {
            item.ProductName = ProductHelper.GetProductName(item.ProductCode, culture);
        }

        return productList;
    }

    private string[] GetAllowedProducts(string functionName = null)
    {
        const string functionAllowedProductsKey = "FunctionAllowedProducts";
        const string FunctionAllowedProductFilePath = "/App_Data/FunctionAllowedProduct.xml";

        var functions = cacheProvider.GetValueFromCache<IEnumerable<XElement>>(functionAllowedProductsKey);
        if (functions == null)
        {
            var filePath = Path.GetFullPath(Path.Join(_webHostEnvironment.ContentRootPath, FunctionAllowedProductFilePath));
            functions = XDocument.Load(filePath).Descendants(nameof(Function));

            if (functions != null && functions.Any())
            {
                cacheProvider.GetOrCreateFromCache(
                    functionAllowedProductsKey,
                    functions,
                    TimeSpan.FromDays(1));
            }
        }

        var configFunctionName = string.IsNullOrWhiteSpace(functionName) ? "_Default" : functionName;
        var config = functions.FirstOrDefault(x => x.Attribute("Name").Value.Equals(configFunctionName, StringComparison.OrdinalIgnoreCase));

        return config == null ? Array.Empty<string>() : config.Attribute(nameof(Product)).Value.Split(',')
                .Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
    }
}