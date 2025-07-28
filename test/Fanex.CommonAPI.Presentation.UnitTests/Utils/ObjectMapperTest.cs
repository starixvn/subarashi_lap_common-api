using AutoFixture;
using AutoFixture.Xunit2;
using Fanex.CommonAPI.Presentation.DTOs.Request;
using Fanex.CommonAPI.Presentation.Utils;
using FluentAssertions;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Utils;

public class ObjectMapperTest
{
    private readonly IFixture _fixture;

    public ObjectMapperTest()
    {
        _fixture = new Fixture();
    }

    [Theory, AutoData]
    public void MapGetProductAccessQuery_ShouldMapAllProperties(ProductAccessRequest request)
    {
        // Act
        var result = ObjectMapper.MapGetProductAccessQuery(request);

        // Assert
        result.LicenseeId.Should().Be(request.LicenseeId);
        result.FunctionKey.Should().Be(request.FunctionKey);
        result.Language.Should().Be(request.Language);
    }

    [Fact]
    public void MapGetProductAccessQuery_ShouldHandleNullValues()
    {
        // Arrange
        var request = _fixture.Build<ProductAccessRequest>()
            .With(x => x.FunctionKey, (string)null)
            .With(x => x.Language, (string)null)
            .Create();

        // Act
        var result = ObjectMapper.MapGetProductAccessQuery(request);

        // Assert
        result.LicenseeId.Should().Be(request.LicenseeId);
        result.FunctionKey.Should().Be(string.Empty);
        result.Language.Should().Be(string.Empty);
    }
}