using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Todos.GetTodos;

using static Endpoint;

public sealed class Handler(ITodoRepository todoRepository)
    : IHandler<Request, Results<Ok<IEnumerable<Response>>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Ok<IEnumerable<Response>>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var todos = await todoRepository.GetAllTodosAsync(cancellationToken);

        var response = todos.Select(todo => new Response(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Type,
            todo.Steps.Select(s => new Response.Step(s.Id, s.Title, s.IsCompleted, s.Order)),
            todo.Tags.Select(t => new Response.Tag(t.Id, t.Name))));

        return TypedResults.Ok(response);
    }
}