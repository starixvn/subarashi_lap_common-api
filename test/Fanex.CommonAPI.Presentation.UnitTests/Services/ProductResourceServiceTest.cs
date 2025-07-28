using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Fanex.CommonAPI.Presentation.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Services;

public class ProductResourceServiceTest
{
    private readonly IFixture _fixture;
    private readonly ICacheService _cacheService;
    private readonly ResourceManager _resourceManager;
    private readonly Dictionary<string, string> _initialProductMap;
    private readonly ProductResourceService _sut;

    public ProductResourceServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _cacheService = Substitute.For<ICacheService>();
        _resourceManager = Substitute.For<ResourceManager>("ViewResources.Product", typeof(ProductResourceServiceTest).Assembly);
        _initialProductMap = new Dictionary<string, string>
        {
            { "1", "Initial Product 1" }
        };
        _sut = new ProductResourceService(_cacheService, _resourceManager, _initialProductMap);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("vi")]
    [InlineData("th")]
    public void GetProductResources_ShouldUseCacheService(string language)
    {
        // Arrange
        var cacheKey = $"ProductResources_{language}";
        var expectedResult = _fixture.Create<Dictionary<string, string>>();
        _cacheService.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<System.Func<Dictionary<string, string>>>(),
            null)
            .Returns(expectedResult);

        // Act
        var result = _sut.GetProductResources(language);

        // Assert
        result.Should().BeSameAs(expectedResult);
        _cacheService.Received(1).GetOrCreateFromCache(
            cacheKey,
            Arg.Any<System.Func<Dictionary<string, string>>>(),
            null);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("vi")]
    [InlineData("th")]
    public void GetProductResources_ShouldMergeInitialMapWithResourceSet(string language)
    {
        // Arrange
        var cacheKey = $"ProductResources_{language}";
        var resources = new Dictionary<string, string>
        {
            { "Product2", "Product 2 Value" },
            { "Product3", "Product 3 Value" },
            { "NotAProduct", "Invalid Value" }
        };
        var resourceSet = new TestResourceSet(resources);

        _resourceManager.GetResourceSet(Arg.Any<CultureInfo>(), true, true)
            .Returns(resourceSet);

        _resourceManager.GetString("Product2", Arg.Any<CultureInfo>())
            .Returns("Product 2 Value");
        _resourceManager.GetString("Product3", Arg.Any<CultureInfo>())
            .Returns("Product 3 Value");
        _resourceManager.GetString("NotAProduct", Arg.Any<CultureInfo>())
            .Returns("Invalid Value");

        var expectedResult = new Dictionary<string, string>(_initialProductMap)
        {
            { "2", "Product 2 Value" },
            { "3", "Product 3 Value" }
        };

        _cacheService.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<System.Func<Dictionary<string, string>>>(),
            null)
            .Returns(expectedResult);

        // Act
        var result = _sut.GetProductResources(language);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("vi")]
    [InlineData("th")]
    public void GetProductResources_ShouldSkipEmptyResourceValues(string language)
    {
        // Arrange
        var cacheKey = $"ProductResources_{language}";
        var resources = new Dictionary<string, string>
        {
            { "Product2", string.Empty },
            { "Product3", null }
        };
        var resourceSet = new TestResourceSet(resources);

        _resourceManager.GetResourceSet(Arg.Any<CultureInfo>(), true, true)
            .Returns(resourceSet);

        _resourceManager.GetString(Arg.Any<string>(), Arg.Any<CultureInfo>())
            .Returns(string.Empty);

        _cacheService.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<System.Func<Dictionary<string, string>>>(),
            null)
            .Returns(_initialProductMap);

        // Act
        var result = _sut.GetProductResources(language);

        // Assert
        result.Should().BeEquivalentTo(_initialProductMap);
    }
}