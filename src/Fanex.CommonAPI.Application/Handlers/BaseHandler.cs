namespace Fanex.CommonAPI.Application.Handlers;

using System.Reflection;
using Domain.Errors;
using Domain.Results;
using Logging;
using MediatR;
using Utils;

public abstract class BaseHandler<TRequest, TResult>(ILogger logger) : IRequestHandler<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : ServiceResult
{
    public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await DoWork(request, cancellationToken);
        }
        catch (Exception exception)
        {
            await logger.ErrorAsync(ErrorMessageUtils.GenerateMessage(exception));

            return CreateFailureResult();
        }
    }

    protected abstract Task<TResult> DoWork(TRequest request, CancellationToken cancellationToken);

    private static TResult CreateFailureResult()
    {
        Type resultType = typeof(TResult);

        var failureMethod = resultType.GetMethod("Failure", BindingFlags.Public | BindingFlags.Static);

        if (failureMethod == null)
        {
            throw new InvalidOperationException($"Could not invoke Failure method on {resultType.Name}");
        }

        return failureMethod.Invoke(null, [ServiceErrors.SystemError, null]) as TResult;
    }
}