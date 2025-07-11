using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Resources;
using Fanex.CommonAPI.Application.Services;
using Fanex.CommonAPI.Domain.Results;
using Fanex.Logging;
using System.Globalization;

namespace Fanex.CommonAPI.Application.Handlers;

public class GetProductResourceByIdHandler : BaseHandler<GetProductResourceByIdQuery, ServiceResult<string>>
{
    private readonly ICacheProvider _cacheProvider;

    public GetProductResourceByIdHandler(
        ICacheProvider cacheProvider,
        ILogger logger) : base(logger)
    {
        _cacheProvider = cacheProvider;
    }

    protected override async Task<ServiceResult<string>> DoWork(GetProductResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.ProductResourcesById(request);

        var result = _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            () => GetResult(request.Language, request.ProductId),
            TimeSpan.FromDays(1));

        return ServiceResult<string>.Success(result);
    }

    private string GetResult(string language, int productId)
    {
        var culture = new CultureInfo(language);

        var value = ViewResources_ProductById.ResourceManager.GetString($"Product_{productId}", culture) ?? string.Empty;

        return value;
    }
}