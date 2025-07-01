namespace Fanex.CommonAPI.Presentation.DTOs.Response;

using Domain.Entities;
using Interfaces;

public class ProductAccessResponse : IResultResponse<IEnumerable<Product>>
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public IEnumerable<Product> Result { get; set; }
}