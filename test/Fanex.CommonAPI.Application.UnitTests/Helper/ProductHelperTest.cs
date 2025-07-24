using System.Globalization;
using Fanex.CommonAPI.Application.Helper;
using FluentAssertions;
using Xunit;

namespace Fanex.CommonAPI.Application.UnitTests.Helper;

public class ProductHelperTest
{
    [Fact]
    public void LiveReportNoDetailLinkSports_ShouldReturnExpectedSports()
    {
        // Act
        var result = ProductHelper.LiveReportNoDetailLinkSports;

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(162);
        result.Should().Contain(251);
        result.Should().Contain(132);
    }

    [Fact]
    public void ProductNameMap_ShouldContainExpectedMappings()
    {
        // Act
        var map = ProductHelper.ProductNameMap;

        // Assert
        map.Should().NotBeEmpty();
        map.Should().ContainKey("VS180");
        map["VS180"].Should().Be("VirtualSoccer");
        map.Should().ContainKey("VS2197");
        map["VS2197"].Should().Be("VirtualSoccerEuroCup");
    }

    [Theory]
    [InlineData("VS180", "en-US", "Virtual Soccer")]
    [InlineData("VS181", "en-US", "Virtual Horse Racing")]
    [InlineData("123", "en-US", "123")]
    [InlineData("", "en-US", "")]
    [InlineData(null, "en-US", "")]
    public void GetProductName_ShouldReturnExpectedName(string productCode, string culture, string expectedName)
    {
        // Arrange
        var cultureInfo = new CultureInfo(culture);

        // Act
        var result = ProductHelper.GetProductName(productCode, cultureInfo);

        // Assert
        result.Should().Be(expectedName);
    }
}