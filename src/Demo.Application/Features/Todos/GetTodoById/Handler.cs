using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Todos.GetTodoById;

using static Endpoint;

public sealed class Handler(ITodoRepository todoRepository)
    : IHandler<Request, Results<Ok<Response>, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Ok<Response>, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetByIdAsync(request.Id, cancellationToken);

        if (todo is null)
            return TypedResults.NotFound(new ProblemDetails
            {
                Detail = $"Todo with ID {request.Id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Todo Not Found"
            });

        var response = new Response(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Type,
            todo.Steps.Select(s => new Response.Step(s.Id, s.Title, s.IsCompleted, s.Order)),
            todo.Tags.Select(t => new Response.Tag(t.Id, t.Name)));

        return TypedResults.Ok(response);
    }
}