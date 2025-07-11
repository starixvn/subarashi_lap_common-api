using Fanex.CommonAPI.Domain.Results;
using MediatR;

namespace Fanex.CommonAPI.Application.Queries;

public class GetProductResourceByIdQuery : IRequest<ServiceResult<string>>
{
    public int ProductId { get; set; }
    public string Language { get; set; }
}