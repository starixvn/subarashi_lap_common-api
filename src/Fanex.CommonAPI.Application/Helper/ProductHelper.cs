using Fanex.CommonAPI.Application.Resources;
using System.Collections.Immutable;
using System.Globalization;

namespace Fanex.CommonAPI.Application.Helper
{
    public static class ProductHelper
    {
        public static int[] LiveReportNoDetailLinkSports =>
        [
            162, 251, 252, 253, 202, 203, 208, 209, 210, 211, 212, 220, 248, 165, 247, 245, 222, 223, 243,
            200, 244, 232, 234, 238, 233, 239, 171, 167, 172, 174, 176, 237, 158, 159, 155, 156, 228, 227,
            216, 230, 146, 229, 138, 148, 145, 144, 143, 130, 131, 139, 124, 129, 123, 136, 121, 122, 132,
            128
        ];

        public static readonly ImmutableDictionary<string, string> ProductNameMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "VS180", nameof(ViewResources_Product.VirtualSoccer) },
                { "VS181", nameof(ViewResources_Product.VirtualHorseRacing) },
                { "VS182", nameof(ViewResources_Product.VirtualGreyhounds) },
                { "VS183", nameof(ViewResources_Product.VirtualSpeedway) },
                { "VS184", nameof(ViewResources_Product.VirtualMotoRacing) },
                { "VS185", nameof(ViewResources_Product.VirtualCycling) },
                { "VS186", nameof(ViewResources_Product.VirtualTennis) },
                { "VS188", nameof(ViewResources_Product.VirtualCockFighting) },
                { "VS2190", nameof(ViewResources_Product.VirtualSoccerLeague) },
                { "VS2191", nameof(ViewResources_Product.VirtualSoccerNation) },
                { "VS2192", nameof(ViewResources_Product.VirtualSoccerWorldCup) },
                { "VS2193", nameof(ViewResources_Product.VirtualBasketball) },
                { "VS2194", nameof(ViewResources_Product.VirtualSoccerAsianCup) },
                { "VS2195", nameof(ViewResources_Product.VirtualSoccerEnglishPremier) },
                { "VS2196", nameof(ViewResources_Product.VirtualSoccerChampionsCup) },
                { "VS2197", nameof(ViewResources_Product.VirtualSoccerEuroCup) }
            }.ToImmutableDictionary();

        public static string GetProductName(string productCode, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(productCode))
            {
                return string.Empty;
            }

            if (!ProductNameMap.TryGetValue(productCode, out var resourceKey))
            {
                resourceKey = "Product" + productCode;
            }

            return ViewResources_Product.ResourceManager.GetString(resourceKey, culture) ?? productCode;
        }
    }
}