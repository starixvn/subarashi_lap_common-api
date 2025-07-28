using System.Net;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Domain.Entities;
using Fanex.CommonAPI.Domain.Errors;
using Fanex.CommonAPI.Domain.Results;
using Fanex.CommonAPI.Presentation.DTOs.Response;
using Fanex.CommonAPI.Presentation.Endpoints;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Endpoints;

public class ProductSettingEndpointsTest
{
    private readonly IFixture _fixture;
    private readonly IMediator _mediator;
    private readonly HttpContext _httpContext;

    public ProductSettingEndpointsTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _mediator = Substitute.For<IMediator>();
        _httpContext = Substitute.For<HttpContext>();
    }

    [Theory, AutoData]
    public async Task GetProductSettings_ShouldReturnOkWithResult_WhenSuccessful(ProductSetting expectedResult)
    {
        // Arrange
        _mediator.Send(
            Arg.Any<GetProductSettingQuery>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<ProductSetting>.Success(expectedResult));

        // Act
        var result = await ProductSettingEndpoints.GetProductSettings(
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductSettingResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(0);
        okResult.Value.ErrorMessage.Should().BeEmpty();
        okResult.Value.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetProductSettings_ShouldReturnOkWithError_WhenFailed()
    {
        // Arrange
        var error = ServiceErrors.SystemError;
        _mediator.Send(
            Arg.Any<GetProductSettingQuery>(),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<ProductSetting>.Failure(error));

        // Act
        var result = await ProductSettingEndpoints.GetProductSettings(
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductSettingResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(error.Code);
        okResult.Value.ErrorMessage.Should().Be(error.Message);
        okResult.Value.Result.LiveReportNoDetailLinkSports.Should().BeEmpty();
    }
}