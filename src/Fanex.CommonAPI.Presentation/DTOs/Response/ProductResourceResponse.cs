using System.Collections.Generic;
using Fanex.CommonAPI.Presentation.DTOs.Interfaces;

namespace Fanex.CommonAPI.Presentation.DTOs.Response;

public class ProductResourceResponse : IResultResponse<Dictionary<string, string>>
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public Dictionary<string, string> Result { get; set; }
}