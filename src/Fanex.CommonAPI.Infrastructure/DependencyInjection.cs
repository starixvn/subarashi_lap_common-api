namespace Fanex.CommonAPI.Infrastructure;

using Application.Services;
using DbSettingConfig;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repositories;
using Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache(options => { options.TrackStatistics = true; });

        services.TryAddSingleton<IShardDbInfoAccessor, ShardDbInfoAccessor>();
        // repositories

        services.TryAddScoped<IProductRepository, ProductRepository>();
        services.TryAddScoped<IServerDbConfigRepository, ServerDbConfigRepository>();
        services.TryAddScoped<ICacheProvider, CacheProvider>();
        services.TryAddSingleton<IDbFactory, DbFactory>();

        return services;
    }
}