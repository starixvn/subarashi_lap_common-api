namespace Fanex.CommonAPI.Domain.Repositories;

using Entities;
using Params;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductAccesssAsync(int licenseeId);
}