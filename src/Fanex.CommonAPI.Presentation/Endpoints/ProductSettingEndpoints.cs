using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Fanex.CommonAPI.Presentation.DTOs.Response;
using Fanex.CommonAPI.Presentation.Extensions;
using MediatR;
using Fanex.CommonAPI.Application.Queries;
using Fanex.CommonAPI.Domain.Entities;

namespace Fanex.CommonAPI.Presentation.Endpoints;

public class ProductSettingEndpoints : IEndpoints
{
    public string GroupName => "ProductSetting";

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(this);
        group.MapGet(GetProductSettings, "");
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ProductSettingResponse), (int)HttpStatusCode.OK)]
    internal static async Task<IResult> GetProductSettings(
        HttpContext httpContext,
        IMediator mediator)
    {
        var query = new GetProductSettingQuery();
        var result = await mediator.Send(query);

        if (result.IsSuccess)
        {
            return Results.Ok(new ProductSettingResponse
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Result = result.Result
            });
        }

        return Results.Ok(new ProductSettingResponse
        {
            ErrorCode = result.Error.Code,
            ErrorMessage = result.Error.Message,
            Result = new ProductSetting
            {
                LiveReportNoDetailLinkSports = []
            }
        });
    }
}