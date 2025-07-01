using MediatR;
using System.Collections.Generic;
using Fanex.CommonAPI.Domain.Results;

namespace Fanex.CommonAPI.Application.Queries;

public class GetProductResourceQuery : IRequest<ServiceResult<Dictionary<string, string>>>
{
    public string Language { get; set; }
    public string ProductCode { get; set; }
}