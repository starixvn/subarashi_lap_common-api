using Fanex.CommonAPI.Presentation.DTOs.Interfaces;

namespace Fanex.CommonAPI.Presentation.DTOs.Response;

public class ProductResourceResponse : IResultResponse<object>
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public object Result { get; set; }
}