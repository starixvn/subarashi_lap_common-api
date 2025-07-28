using System.Globalization;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Handlers;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Services;
using Fanex.Logging;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Application.UnitTests.Handlers;

public class GetProductResourceByIdHandlerTest
{
    private readonly IFixture _fixture;
    private readonly ILogger _logger;
    private readonly ICacheProvider _cacheProvider;
    private readonly GetProductResourceByIdHandler _sut;

    public GetProductResourceByIdHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _logger = Substitute.For<ILogger>();
        _cacheProvider = Substitute.For<ICacheProvider>();
        _sut = new GetProductResourceByIdHandler(_cacheProvider, _logger);
    }

    [Theory]
    [InlineData("en", 123)]
    [InlineData("vi", 456)]
    public async Task Handle_ShouldReturnCachedResource_WhenCacheExists(string language, int productId)
    {
        // Arrange
        var query = new GetProductResourceByIdQuery { Language = language, ProductId = productId };
        var cacheKey = CacheKeys.ProductResourcesById(query);
        var expectedResult = _fixture.Create<string>();
        _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<Func<string>>(),
            Arg.Any<TimeSpan>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("en", 123)]
    [InlineData("vi", 456)]
    public async Task Handle_ShouldReturnEmptyString_WhenResourceNotFound(string language, int productId)
    {
        // Arrange
        var query = new GetProductResourceByIdQuery { Language = language, ProductId = productId };
        var cacheKey = CacheKeys.ProductResourcesById(query);

        _cacheProvider.GetOrCreateFromCache(
            cacheKey,
            Arg.Any<Func<string>>(),
            Arg.Any<TimeSpan>())
            .Returns(info =>
            {
                var factory = info.Arg<Func<string>>();
                return factory();
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("en", 123)]
    [InlineData("vi", 456)]
    public async Task Handle_ShouldHandleException(string language, int productId)
    {
        // Arrange
        var query = new GetProductResourceByIdQuery { Language = language, ProductId = productId };
        _cacheProvider.When(x => x.GetOrCreateFromCache(
            Arg.Any<string>(),
            Arg.Any<Func<string>>(),
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