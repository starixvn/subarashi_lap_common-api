namespace Fanex.CommonAPI.Presentation.Filters;

using Microsoft.AspNetCore.Http;

public class ValidatorProductAccessFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var licenseeId = context.HttpContext.Request.Query["licenseeId"].ToString();

        if (!Int32.TryParse(licenseeId, out int licenseIdValue) || licenseIdValue <= 0)
        {
            return Results.BadRequest("Invalid licenseeId parameter");
        }

        return await next(context);
    }
}