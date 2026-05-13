using Microsoft.AspNetCore.Http;

namespace Demo.Application.Common.Interfaces;

public interface IHandler<in TRequest, TResult> where TResult : IResult
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}