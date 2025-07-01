namespace Fanex.CommonAPI.Infrastructure.DbSettingConfig;

using Application.Utils;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

public class ShardDbInfoAccessor(IHttpContextAccessor httpContextAccessor) : IShardDbInfoAccessor
{
    private readonly string _prefixShardCustDB = "ShardCustDB_";
    private readonly string _prefixShardSiteDB = "ShardSiteDB_";

    private ServerDbConfig ServerDbConfig => ServerDbConfigUtils.GetServerDbConfigFromHttpContext(httpContextAccessor);

    public string GetSpKey(string key)
        => key
            .Replace(_prefixShardCustDB, string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(_prefixShardSiteDB, string.Empty, StringComparison.OrdinalIgnoreCase);

    public string ToShardCustDbSp(string key) => $"{_prefixShardCustDB}{key}";

    public string ToShardSiteDbSp(string key) => $"{_prefixShardSiteDB}{key}";

    public bool TryUseShardDb(string key, out string dbServer)
    {
        if (IsShardSiteDb(key) && !string.IsNullOrEmpty(ServerDbConfig.SiteDbServer))
        {
            dbServer = ServerDbConfig.SiteDbServer;
            return true;
        }

        if (IsShardCustDb(key) && !string.IsNullOrEmpty(ServerDbConfig.CustDbServer))
        {
            dbServer = ServerDbConfig.CustDbServer;
            return true;
        }

        dbServer = null;
        return false;
    }

    private bool IsShardCustDb(string key) => key.StartsWith(_prefixShardCustDB, StringComparison.OrdinalIgnoreCase);

    private bool IsShardSiteDb(string key) => key.StartsWith(_prefixShardSiteDB, StringComparison.OrdinalIgnoreCase);
}