namespace Fanex.CommonAPI.Domain.Repositories;

using Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductAccesssAsync(int licenseeId);
}