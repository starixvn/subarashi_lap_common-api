namespace Fanex.CommonAPI.Application.Constants;

using Queries;

public static class CacheKeys
{
    public static Func<GetProductAccessQuery, string> ProductAccess => query => $"ProductAccess_{query.LicenseeId}_{query.FunctionKey}_{query.Language}";

    public static Func<GetProductResourceQuery, string> ProductResources => query => $"ProductResources_{query.Language}_{query.ProductCode}";
}