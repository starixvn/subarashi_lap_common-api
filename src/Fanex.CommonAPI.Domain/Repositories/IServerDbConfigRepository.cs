namespace Fanex.CommonAPI.Domain.Repositories;

using Entities;

public interface IServerDbConfigRepository
{
    Task<ServerDbConfig> GetServerDbConfigByAgentIdAsync(int agentId);
}