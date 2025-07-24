namespace Fanex.CommonAPI.Application.Queries;

using Domain.Entities;
using Domain.Results;
using MediatR;
using System.Collections.Generic;

public class GetProductAccessQuery : IRequest<ServiceResult<IEnumerable<Product>>>
{
    public int LicenseeId { get; set; }

    public string FunctionKey { get; set; }

    public string Language { get; set; }
}