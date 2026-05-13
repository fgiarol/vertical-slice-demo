using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Steps.GetSteps;

using static Endpoint;

public sealed class Handler(ITodoRepository todoRepository)
    : IHandler<Request, Results<Ok<IEnumerable<Response>>, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<
        Ok<IEnumerable<Response>>, 
        NotFound<ProblemDetails>, 
        InternalServerError<ProblemDetails>>> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Todo not found",
                Detail = $"Todo with id {request.TodoId} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var response = todo.Steps.Select(s => new Response(
            s.Id,
            s.Title,
            s.IsCompleted,
            s.Order));

        return TypedResults.Ok(response);
    }
}