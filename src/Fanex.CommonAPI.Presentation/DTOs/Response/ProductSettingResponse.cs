using Fanex.CommonAPI.Domain.Entities;
using Fanex.CommonAPI.Presentation.DTOs.Interfaces;

namespace Fanex.CommonAPI.Presentation.DTOs.Response;

public class ProductSettingResponse : IResultResponse<ProductSetting>
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public ProductSetting Result { get; set; }
}