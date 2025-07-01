namespace Fanex.CommonAPI.Presentation.UnitTests.Utils;

using AutoFixture;
using DTOs.Request;
using FluentAssertions;
using Presentation.Utils;

public class ObjectMapperTest
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void MapGetProductAccessQuery_Should_MapPropertiesCorrectly()
    {
        // Arrange
        var request = _fixture.Create<ProductAccessRequest>();

        // Action
        var getProductAccessQuery = ObjectMapper.MapGetProductAccessQuery(request);

        // Assert
        getProductAccessQuery.LicenseeId.Should().Be(request.LicenseeId);
        getProductAccessQuery.FunctionKey.Should().Be(request.FunctionKey);
    }
}