using System.Collections;
using System.Globalization;
using System.Resources;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Handlers;
using Fanex.CommonAPI.Application.Helper;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Services;
using Fanex.Logging;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Application.UnitTests.Handlers;

public class GetProductResourceHandlerTest
{
    private readonly IFixture _fixture;
    private readonly ILogger _logger;
    private readonly ICacheProvider _cacheProvider;
    private readonly GetProductResourceHandler _sut;

    public GetProductResourceHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _logger = Substitute.For<ILogger>();
        _cacheProvider = Substitute.For<ICacheProvider>();
        _sut = new GetProductResourceHandler(_cacheProvider, _logger);
    }

    [Theory]
    [InlineData("en", "VS180")]
    [InlineData("vi", "VS181")]
    public async Task Handle_ShouldReturnCachedResource_WhenCacheExists(string language, string productCode)
    {
        // Arrange
        var query = new GetProductResourceQuery { Language = language, ProductCode = productCode };
        var cacheKey = CacheKeys.ProductResources(query);
        var expectedResult = _fixture.Create<string>();
        _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<Func<object>>(),
            Arg.Any<TimeSpan>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("en", "VS180")]
    [InlineData("vi", "VS181")]
    public async Task Handle_ShouldReturnSingleResource_WhenProductCodeIsProvided(string language, string productCode)
    {
        // Arrange
        var query = new GetProductResourceQuery { Language = language, ProductCode = productCode };
        var cacheKey = CacheKeys.ProductResources(query);

        _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<Func<object>>(),
            Arg.Any<TimeSpan>())
            .Returns(info =>
            {
                var factory = info.Arg<Func<object>>();
                return factory();
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeOfType<string>();
    }

    [Theory]
    [InlineData("en")]
    [InlineData("vi")]
    public async Task Handle_ShouldReturnAllResources_WhenProductCodeIsNotProvided(string language)
    {
        // Arrange
        var query = new GetProductResourceQuery { Language = language };
        var cacheKey = CacheKeys.ProductResources(query);

        _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<Func<object>>(),
            Arg.Any<TimeSpan>())
            .Returns(info =>
            {
                var factory = info.Arg<Func<object>>();
                return factory();
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeOfType<Dictionary<string, string>>();
    }

    [Theory]
    [InlineData("en", "VS180")]
    [InlineData("vi", "VS181")]
    public async Task Handle_ShouldHandleException(string language, string productCode)
    {
        // Arrange
        var query = new GetProductResourceQuery { Language = language, ProductCode = productCode };
        _cacheProvider.When(x => x.GetOrCreateFromCache(
            Arg.Any<string>(),
            Arg.Any<Func<object>>(),
            Arg.Any<TimeSpan>()))
            .Do(x => { throw new Exception("Test exception"); });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(999);
        await _logger.Received(1).ErrorAsync(Arg.Any<string>());
    }
}