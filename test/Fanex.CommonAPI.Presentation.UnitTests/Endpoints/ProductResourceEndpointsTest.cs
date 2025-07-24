using System.Net;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Domain.Errors;
using Fanex.CommonAPI.Domain.Results;
using Fanex.CommonAPI.Presentation.DTOs.Response;
using Fanex.CommonAPI.Presentation.Endpoints;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Endpoints;

public class ProductResourceEndpointsTest
{
    private readonly IFixture _fixture;
    private readonly IMediator _mediator;
    private readonly HttpContext _httpContext;
    private readonly HttpRequest _httpRequest;
    private readonly IHeaderDictionary _headers;

    public ProductResourceEndpointsTest()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _mediator = Substitute.For<IMediator>();
        _httpContext = Substitute.For<HttpContext>();
        _httpRequest = Substitute.For<HttpRequest>();
        _headers = Substitute.For<IHeaderDictionary>();

        _httpRequest.Headers.Returns(_headers);
        _httpContext.Request.Returns(_httpRequest);
    }

    [Theory, AutoData]
    public async Task GetProductResources_ShouldReturnOkWithResult_WhenSuccessful(
        string productCode,
        string language,
        string expectedResult)
    {
        // Arrange
        _headers["Language"].Returns(new StringValues(language));
        _mediator.Send(
            Arg.Is<GetProductResourceQuery>(q =>
                q.ProductCode == productCode &&
                q.Language == language),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<object>.Success(expectedResult));

        // Act
        var result = await ProductResourceEndpoints.GetProductResources(
            productCode,
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductResourceResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(0);
        okResult.Value.ErrorMessage.Should().BeEmpty();
        okResult.Value.Result.Should().Be(expectedResult);
    }

    [Theory, AutoData]
    public async Task GetProductResources_ShouldReturnOkWithError_WhenFailed(
        string productCode,
        string language)
    {
        // Arrange
        var error = ServiceErrors.SystemError;
        _headers["Language"].Returns(new StringValues(language));
        _mediator.Send(
            Arg.Is<GetProductResourceQuery>(q =>
                q.ProductCode == productCode &&
                q.Language == language),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<object>.Failure(error));

        // Act
        var result = await ProductResourceEndpoints.GetProductResources(
            productCode,
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductResourceResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(error.Code);
        okResult.Value.ErrorMessage.Should().Be(error.Message);
        okResult.Value.Result.Should().Be(string.Empty);
    }

    [Theory, AutoData]
    public async Task GetProductResourceById_ShouldReturnOkWithResult_WhenSuccessful(
        int productId,
        string language,
        string expectedResult)
    {
        // Arrange
        _headers["Language"].Returns(new StringValues(language));
        _mediator.Send(
            Arg.Is<GetProductResourceByIdQuery>(q =>
                q.ProductId == productId &&
                q.Language == language),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<string>.Success(expectedResult));

        // Act
        var result = await ProductResourceEndpoints.GetProductResourceById(
            productId,
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductResourceResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(0);
        okResult.Value.ErrorMessage.Should().BeEmpty();
        okResult.Value.Result.Should().Be(expectedResult);
    }

    [Theory, AutoData]
    public async Task GetProductResourceById_ShouldReturnOkWithError_WhenFailed(
        int productId,
        string language)
    {
        // Arrange
        var error = ServiceErrors.SystemError;
        _headers["Language"].Returns(new StringValues(language));
        _mediator.Send(
            Arg.Is<GetProductResourceByIdQuery>(q =>
                q.ProductId == productId &&
                q.Language == language),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<string>.Failure(error));

        // Act
        var result = await ProductResourceEndpoints.GetProductResourceById(
            productId,
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductResourceResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(error.Code);
        okResult.Value.ErrorMessage.Should().Be(error.Message);
        okResult.Value.Result.Should().Be(string.Empty);
    }
}