using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Fanex.CommonAPI.Presentation.DTOs.Response;
using Fanex.CommonAPI.Presentation.Extensions;
using MediatR;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Domain.Extensions;
using System.Collections.Generic;

namespace Fanex.CommonAPI.Presentation.Endpoints;

public class ProductResourceEndpoints : IEndpoints
{
    public string GroupName => "ProductResource";

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(this);
        group.MapGet(GetProductResources, "");
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ProductResourceResponse), (int)HttpStatusCode.OK)]
    internal static async Task<IResult> GetProductResources(
        [FromQuery] string productCode,
        HttpContext httpContext,
        IMediator mediator)
    {
        var query = new GetProductResourceQuery
        {
            ProductCode = productCode,
            Language = httpContext.Request.Headers["Language"].ToString()
        };

        var result = await mediator.Send(query);

        return result.Match(
            _ => Results.Ok(new ProductResourceResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Result = result.Result
            }),
            (error, _) => Results.Ok(new ProductResourceResponse
            {
                ErrorCode = error.Code,
                ErrorMessage = error.Message,
                Result = string.Empty
            })
        );
    }
}