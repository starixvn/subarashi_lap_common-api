using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Fanex.CommonAPI.Application.Constants;
using Fanex.CommonAPI.Application.Handlers;
using Fanex.CommonAPI.Application.Helper;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Application.Services;
using Fanex.CommonAPI.Domain.Entities;
using Fanex.Logging;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Application.UnitTests.Handlers;

public class GetProductSettingHandlerTest
{
    private readonly IFixture _fixture;
    private readonly ILogger _logger;
    private readonly ICacheProvider _cacheProvider;
    private readonly GetProductSettingHandler _sut;

    public GetProductSettingHandlerTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _logger = Substitute.For<ILogger>();
        _cacheProvider = Substitute.For<ICacheProvider>();
        _sut = new GetProductSettingHandler(_cacheProvider, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedSettings_WhenCacheExists()
    {
        // Arrange
        var query = new GetProductSettingQuery();
        var expectedSettings = _fixture.Create<ProductSetting>();
        _cacheProvider.GetValueFromCache<ProductSetting>(CacheKeys.ProductSettings).Returns(expectedSettings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(expectedSettings);
    }

    [Fact]
    public async Task Handle_ShouldReturnNewSettings_WhenCacheDoesNotExist()
    {
        // Arrange
        var query = new GetProductSettingQuery();
        _cacheProvider.GetValueFromCache<ProductSetting>(CacheKeys.ProductSettings).Returns((ProductSetting)null);
        _cacheProvider.GetOrCreateFromCache(
            CacheKeys.ProductSettings,
            Arg.Any<Func<ProductSetting>>(),
            Arg.Any<TimeSpan>())
            .Returns(info =>
            {
                var settings = info.Arg<Func<ProductSetting>>()();
                settings.LiveReportNoDetailLinkSports.Should().BeEquivalentTo(ProductHelper.LiveReportNoDetailLinkSports);
                return settings;
            });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.LiveReportNoDetailLinkSports.Should().BeEquivalentTo(ProductHelper.LiveReportNoDetailLinkSports);
    }

    [Fact]
    public async Task Handle_ShouldHandleException()
    {
        // Arrange
        var query = new GetProductSettingQuery();
        _cacheProvider.When(x => x.GetValueFromCache<ProductSetting>(Arg.Any<string>()))
            .Do(x => { throw new Exception("Test exception"); });

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(999);
        await _logger.Received(1).ErrorAsync(Arg.Any<string>());
    }
}