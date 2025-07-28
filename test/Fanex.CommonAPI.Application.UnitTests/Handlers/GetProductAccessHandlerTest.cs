using System.Globalization;
using System.Xml.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Handlers;
using Fanex.CommonAPI.Application.Helper;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Services;
using Fanex.CommonAPI.Domain.Entities;
using Fanex.CommonAPI.Domain.Repositories;
using Fanex.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Application.UnitTests.Handlers;

public class GetProductAccessHandlerTest
{
    private readonly IFixture _fixture;
    private readonly ILogger _logger;
    private readonly IProductRepository _productRepository;
    private readonly ICacheProvider _cacheProvider;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly GetProductAccessHandler _sut;

    public GetProductAccessHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _logger = Substitute.For<ILogger>();
        _productRepository = Substitute.For<IProductRepository>();
        _cacheProvider = Substitute.For<ICacheProvider>();
        _webHostEnvironment = Substitute.For<IWebHostEnvironment>();

        _sut = new GetProductAccessHandler(_logger, _productRepository, _cacheProvider, _webHostEnvironment);
    }

    [Theory]
    [InlineData(123, "test_function", "en")]
    [InlineData(456, "other_function", "vi")]
    public async Task Handle_ShouldReturnCachedProducts_WhenCacheExists(int licenseeId, string functionKey, string language)
    {
        // Arrange
        var query = new GetProductAccessQuery { LicenseeId = licenseeId, FunctionKey = functionKey, Language = language };
        var cacheKey = CacheKeys.ProductAccess(query);
        var expectedProducts = _fixture.CreateMany<Product>().ToList();
        _cacheProvider.GetValueFromCache<IEnumerable<Product>>(cacheKey).Returns(expectedProducts);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(expectedProducts);
    }

    [Theory]
    [InlineData(123, "test_function", "en")]
    [InlineData(456, "other_function", "vi")]
    public async Task Handle_ShouldReturnFilteredProducts_WhenAllowedProductsExist(int licenseeId, string functionKey, string language)
    {
        // Arrange
        var query = new GetProductAccessQuery { LicenseeId = licenseeId, FunctionKey = functionKey, Language = language };
        var cacheKey = CacheKeys.ProductAccess(query);
        var products = _fixture.CreateMany<Product>(5).ToList();
        var allowedProducts = new[] { products[0].ProductCode, products[1].ProductCode };
        var expectedProducts = products.Where(p => allowedProducts.Contains(p.ProductCode)).ToList();

        _cacheProvider.GetValueFromCache<IEnumerable<Product>>(cacheKey).Returns((IEnumerable<Product>)null);
        _cacheProvider.GetValueFromCache<IEnumerable<XElement>>("FunctionAllowedProducts").Returns((IEnumerable<XElement>)null);

        var xmlContent = XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-8""?>
            <Functions>
                <Function Name=""{query.FunctionKey}"" Product=""{string.Join(",", allowedProducts)}"" />
            </Functions>");

        _webHostEnvironment.ContentRootPath.Returns("/path/to/root");
        _cacheProvider.GetValueFromCache<IEnumerable<XElement>>("FunctionAllowedProducts")
            .Returns(xmlContent.Descendants("Function"));

        _productRepository.GetProductAccesssAsync(query.LicenseeId).Returns(products);

        _cacheProvider.GetOrCreateFromCacheAsync(
            cacheKey,
            Arg.Any<Func<Task<IEnumerable<Product>>>>(),
            Arg.Any<TimeSpan>())
            .Returns(info => info.Arg<Func<Task<IEnumerable<Product>>>>()().Result);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(expectedProducts);
    }

    [Theory]
    [InlineData(123, "test_function", "en")]
    [InlineData(456, "other_function", "vi")]
    public async Task Handle_ShouldReturnAllProducts_WhenNoAllowedProductsExist(int licenseeId, string functionKey, string language)
    {
        // Arrange
        var query = new GetProductAccessQuery { LicenseeId = licenseeId, FunctionKey = functionKey, Language = language };
        var cacheKey = CacheKeys.ProductAccess(query);
        var products = _fixture.CreateMany<Product>(5).ToList();

        _cacheProvider.GetValueFromCache<IEnumerable<Product>>(cacheKey).Returns((IEnumerable<Product>)null);
        _cacheProvider.GetValueFromCache<IEnumerable<XElement>>("FunctionAllowedProducts").Returns((IEnumerable<XElement>)null);

        var xmlContent = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <Functions>
                <Function Name=""OtherFunction"" Product=""VS180,VS181"" />
            </Functions>");

        _webHostEnvironment.ContentRootPath.Returns("/path/to/root");
        _cacheProvider.GetValueFromCache<IEnumerable<XElement>>("FunctionAllowedProducts")
            .Returns(xmlContent.Descendants("Function"));

        _productRepository.GetProductAccesssAsync(query.LicenseeId).Returns(products);

        _cacheProvider.GetOrCreateFromCacheAsync(
            cacheKey,
            Arg.Any<Func<Task<IEnumerable<Product>>>>(),
            Arg.Any<TimeSpan>())
            .Returns(info => info.Arg<Func<Task<IEnumerable<Product>>>>()().Result);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(products);
    }

    [Theory]
    [InlineData(123, "test_function", "en")]
    [InlineData(456, "other_function", "vi")]
    public async Task Handle_ShouldHandleException(int licenseeId, string functionKey, string language)
    {
        // Arrange
        var query = new GetProductAccessQuery { LicenseeId = licenseeId, FunctionKey = functionKey, Language = language };
        _cacheProvider.When(x => x.GetValueFromCache<IEnumerable<Product>>(Arg.Any<string>()))
            .Do(x => { throw new Exception("Test exception"); });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(999);
        await _logger.Received(1).ErrorAsync(Arg.Any<string>());
    }
}