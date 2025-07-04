using AsyncKeyedLock;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Domain.Results;
using Fanex.CommonAPI.Domain.Entities;
using Fanex.CommonAPI.Application.Services;
using Fanex.Logging;
using Fanex.CommonAPI.Application.Helper;

namespace Fanex.CommonAPI.Application.Handlers;

public class GetProductSettingHandler : BaseHandler<GetProductSettingQuery, ServiceResult<ProductSetting>>
{
    private const int Timeout = 500;
    private readonly ICacheProvider _cacheProvider;

    private static readonly AsyncKeyedLocker<string> AsyncKeyedLocker = new(new AsyncKeyedLockOptions
    {
        MaxCount = 1,
        PoolSize = 0
    });

    public GetProductSettingHandler(
        ICacheProvider cacheProvider,
        ILogger logger) : base(logger)
    {
        _cacheProvider = cacheProvider;
    }

    protected override async Task<ServiceResult<ProductSetting>> DoWork(
        GetProductSettingQuery request,
        CancellationToken cancellationToken)
    {
        var result = await GetResult();
        return result;
    }

    private async Task<ServiceResult<ProductSetting>> GetResult()
    {
        var settings = _cacheProvider.GetValueFromCache<ProductSetting>(CacheKeys.ProductSettings);

        if (settings is not null)
        {
            return ServiceResult<ProductSetting>.Success(settings);
        }

        using (await AsyncKeyedLocker.LockOrNullAsync(CacheKeys.ProductSettings, Timeout))
        {
            settings = _cacheProvider
                .GetOrCreateFromCache(
                    CacheKeys.ProductSettings,
                    () => GetProductSettings(),
                    TimeSpan.FromDays(1));

            return ServiceResult<ProductSetting>.Success(settings);
        }
    }

    private static ProductSetting GetProductSettings()
    {
        var settings = new ProductSetting
        {
            LiveReportNoDetailLinkSports = ProductHelper.LiveReportNoDetailLinkSports
        };

        return settings;
    }
}