namespace Fanex.CommonAPI.Presentation.Utils;

using Application.Queries;
using DTOs.Request;

public static class ObjectMapper
{
    public static GetProductAccessQuery MapGetProductAccessQuery(ProductAccessRequest request)
    {
        return new GetProductAccessQuery
        {
            LicenseeId = request.LicenseeId,
            FunctionKey = request.FunctionKey ?? string.Empty,
            Language = request.Language ?? string.Empty
        };
    }
}