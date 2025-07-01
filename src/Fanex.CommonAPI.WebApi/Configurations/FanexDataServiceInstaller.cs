namespace Fanex.CommonAPI.WebApi.Configurations;

using Data;
using Data.SqlServer;
using Data.Vault;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class FanexDataServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        DbSettingProviderManager
            .StartNewSession()
            .WithSqlServer(configuration)
            .UseDefaultDbSettingProvider("~/FanexSpName.xml")
            .Run();

        services.TryAddSingleton<VaultDbAliasService, VaultDbAliasServiceImp>();
    }
}