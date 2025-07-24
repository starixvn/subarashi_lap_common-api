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
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Endpoints;

public class ProductAccessEndpointsTest
{
    private readonly IFixture _fixture;
    private readonly IMediator _mediator;
    private readonly HttpContext _httpContext;
    private readonly HttpRequest _httpRequest;
    private readonly IHeaderDictionary _headers;

    public ProductAccessEndpointsTest()
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
    public async Task GetProductAccesss_ShouldReturnOkWithResult_WhenSuccessful(
        int licenseeId,
        string functionKey,
        string language,
        List<Product> expectedResult)
    {
        // Arrange
        _headers["Language"].Returns(new StringValues(language));
        _mediator.Send(
            Arg.Is<GetProductAccessQuery>(q =>
                q.LicenseeId == licenseeId &&
                q.FunctionKey == functionKey &&
                q.Language == language),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<IEnumerable<Product>>.Success(expectedResult));

        // Act
        var result = await ProductAccessEndpoints.GetProductAccesss(
            licenseeId,
            functionKey,
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductAccessResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(0);
        okResult.Value.ErrorMessage.Should().BeEmpty();
        okResult.Value.Result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory, AutoData]
    public async Task GetProductAccesss_ShouldReturnOkWithError_WhenFailed(
        int licenseeId,
        string functionKey,
        string language)
    {
        // Arrange
        var error = ServiceErrors.SystemError;
        _headers["Language"].Returns(new StringValues(language));
        _mediator.Send(
            Arg.Is<GetProductAccessQuery>(q =>
                q.LicenseeId == licenseeId &&
                q.FunctionKey == functionKey &&
                q.Language == language),
            Arg.Any<CancellationToken>())
            .Returns(ServiceResult<IEnumerable<Product>>.Failure(error));

        // Act
        var result = await ProductAccessEndpoints.GetProductAccesss(
            licenseeId,
            functionKey,
            _httpContext,
            _mediator);

        // Assert
        var okResult = result.Should().BeOfType<Ok<ProductAccessResponse>>().Subject;
        okResult.Value.ErrorCode.Should().Be(error.Code);
        okResult.Value.ErrorMessage.Should().Be(error.Message);
        okResult.Value.Result.Should().BeEmpty();
    }
}