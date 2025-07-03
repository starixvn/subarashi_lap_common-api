namespace Fanex.CommonAPI.Infrastructure.Repositories;

using Constants;
using Domain.Entities;
using Domain.Params;
using Domain.Repositories;
using Utils;
using System.Collections.Generic;
using Fanex.Data;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Helper;

public class ProductRepository(IDbFactory dbFactory) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductAccesssAsync(int licenseeId)
    {
        var allowedProducts = new List<Product> {
            new() { IsOfferedSingleWallet = true, ProductCode = ProductCode.SB },
            new() { IsOfferedSingleWallet = true, ProductCode = ProductCode.Parlay }
        };

        const string spName = "Lrf_LicenseeProduct3rdSel";

        using var objectDb = dbFactory.GetObjectDb(spName);

        objectDb.SetParameterValue(DbParam.LicenseeId, licenseeId);

        using var reader = await objectDb.ExecuteReaderAsync();

        while (reader.Read())
        {
            allowedProducts.Add(new Product
            {
                ProductID = Convert.ToInt32(reader["ProductID"]),
                ProductCode = Convert.ToString(reader["productprefix"]),
                AllowCommission = Convert.ToBoolean(reader["comm"]),
                AllowWinloss = Convert.ToBoolean(reader["winloss"]),
                IsOfferedSingleWallet = Convert.ToBoolean(reader["IsOfferedSingleWallet"])
            });
        }

        return allowedProducts;
    }
}