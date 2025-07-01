namespace Fanex.CommonAPI.WebApi.Configurations;

public interface IServiceInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration);
}