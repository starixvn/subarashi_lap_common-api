namespace Fanex.CommonAPI.Presentation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Fanex.CommonAPI.Presentation.Services;
using System.Resources;
using System.Collections.Generic;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.TryAddScoped<ICacheService, CacheService>();

        services.TryAddScoped<IProductResourceService>(sp =>
        {
            var resourceManager = new ResourceManager(
                "Fanex.CommonAPI.Presentation.Resources.ViewResources",
                typeof(DependencyInjection).Assembly);

            var initialProductMap = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                { "VS180", "VirtualSoccer" },
                { "VS181", "VirtualHorseRacing" },
                { "VS182", "VirtualGreyhounds" },
                { "VS183", "VirtualSpeedway" },
                { "VS184", "VirtualMotoRacing" },
                { "VS185", "VirtualCycling" },
                { "VS186", "VirtualTennis" },
                { "VS188", "VirtualCockFighting" },
                { "VS2190", "VirtualSoccerLeague" },
                { "VS2191", "VirtualSoccerNation" },
                { "VS2192", "VirtualSoccerWorldCup" },
                { "VS2193", "VirtualBasketball" },
                { "VS2194", "VirtualSoccerAsianCup" },
                { "VS2195", "VirtualSoccerEnglishPremier" },
                { "VS2196", "VirtualSoccerChampionsCup" },
                { "VS2197", "VirtualSoccerEuroCup" }
            };

            return new ProductResourceService(
                sp.GetRequiredService<ICacheService>(),
                resourceManager,
                initialProductMap);
        });

        return services;
    }
}