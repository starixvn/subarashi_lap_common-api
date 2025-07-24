using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Fanex.CommonAPI.Application.Services;
using Fanex.CommonAPI.Presentation.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Services;

public class CacheServiceTest
{
    private readonly IFixture _fixture;
    private readonly ICacheProvider _cacheProvider;
    private readonly CacheService _sut;

    public CacheServiceTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _cacheProvider = Substitute.For<ICacheProvider>();
        _sut = new CacheService(_cacheProvider);
    }

    [Theory, AutoData]
    public void GetOrCreateFromCache_ShouldCallCacheProvider(string key, string value, TimeSpan cacheTime)
    {
        // Arrange
        Func<string> factory = () => value;

        // Act
        _sut.GetOrCreateFromCache(key, factory, cacheTime);

        // Assert
        _cacheProvider.Received(1).GetOrCreateFromCache(key, factory, cacheTime);
    }

    [Theory, AutoData]
    public void GetOrCreateFromCache_ShouldReturnValueFromCacheProvider(string key, string expectedValue)
    {
        // Arrange
        Func<string> factory = () => expectedValue;
        _cacheProvider.GetOrCreateFromCache(key, factory, null).Returns(expectedValue);

        // Act
        var result = _sut.GetOrCreateFromCache(key, factory);

        // Assert
        result.Should().Be(expectedValue);
    }
}