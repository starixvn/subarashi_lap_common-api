namespace Fanex.CommonAPI.Infrastructure.UnitTests.Repositories;

using AutoFixture;
using Constants;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Repositories;
using NSubstitute;
using System.Data;
using Fanex.CommonAPI.Application.Constants;

[Collection("DbFactoryFixture")]
public class ProductRepositoryTest(DbFactoryFixture dbFactoryFixture) : RepositoryBaseTest(dbFactoryFixture)
{
    private readonly IProductRepository _productRepository = new ProductRepository(dbFactoryFixture);

    [Fact]
    public async Task GetProductAccesssAsync_Should_SetParameterValueCorrect()
    {
        // Arrange
        var licenseeId = Fixture.Create<int>();

        // Action
        await _productRepository.GetProductAccesssAsync(licenseeId);

        // Assert
        ObjectDb.Received(1).SetParameterValue(DbParam.LicenseeId, licenseeId);
    }

    [Fact]
    public async Task GetProductAccesssAsync_Should_ReturnCorrectData()
    {
        // Arrange
        var licenseeId = Fixture.Create<int>();
        var expectedProducts = Fixture.CreateMany<Product>(3).ToList();
        var defaultProduct = new Product { IsOfferedSingleWallet = true, ProductCode = ProductCode.SB };
        expectedProducts.Insert(0, defaultProduct);

        var dataTable = new DataTable();
        dataTable.Columns.Add("ProductID", typeof(int));
        dataTable.Columns.Add("productprefix", typeof(string));
        dataTable.Columns.Add("comm", typeof(bool));
        dataTable.Columns.Add("winloss", typeof(bool));
        dataTable.Columns.Add("IsOfferedSingleWallet", typeof(bool));

        foreach (var product in expectedProducts.Skip(1))
        {
            dataTable.Rows.Add(
                product.ProductID,
                product.ProductCode,
                product.AllowCommission,
                product.AllowWinloss,
                product.IsOfferedSingleWallet);
        }

        ObjectDb.ExecuteReaderAsync().Returns(dataTable.CreateDataReader());

        // Action
        var result = await _productRepository.GetProductAccesssAsync(licenseeId);

        // Assert
        result.Should().BeEquivalentTo(expectedProducts,
            options => options.Excluding(p => p.ProductName));
    }
}