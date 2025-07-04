namespace Fanex.CommonAPI.Presentation.Endpoints;

using System.Net;
using Domain.Entities;
using Domain.Extensions;
using DTOs.Request;
using DTOs.Response;
using Extensions;
using Filters;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Utils;
using System.Collections.Generic;

public class ProductAccessEndpoints : IEndpoints
{
    public string GroupName => "ProductAccess";

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(this);

        group
            .MapGet(GetProductAccesss, "")
            .AddEndpointFilter<ValidatorProductAccessFilter>();
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ProductAccessResponse), (int)HttpStatusCode.OK)]
    internal static async Task<IResult> GetProductAccesss(
        [FromQuery] int licenseeId,
        [FromQuery] string functionKey,
        HttpContext httpContext,
        IMediator mediator)
    {
        var request = new ProductAccessRequest
        {
            LicenseeId = licenseeId,
            FunctionKey = functionKey,
            Language = httpContext.Request.Headers["Language"].ToString()
        };

        var query = ObjectMapper.MapGetProductAccessQuery(request);
        var result = await mediator.Send(query);

        return result.Match(
            _ => Results.Ok(new ProductAccessResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Result = result.Result
            }),
            (error, _) => Results.Ok(new ProductAccessResponse
            {
                ErrorCode = error.Code,
                ErrorMessage = error.Message,
                Result = new List<Product>()
            })
        );
    }
}