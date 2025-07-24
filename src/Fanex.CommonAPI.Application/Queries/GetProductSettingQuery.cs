using MediatR;
using Fanex.CommonAPI.Domain.Results;
using Fanex.CommonAPI.Domain.Entities;

namespace Fanex.CommonAPI.Application.Queries;

public class GetProductSettingQuery : IRequest<ServiceResult<ProductSetting>>
{
}