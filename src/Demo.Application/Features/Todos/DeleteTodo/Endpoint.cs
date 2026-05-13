using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Todos.DeleteTodo;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Todos";
    private const string BaseRoute = "todos/{id:guid}";

    public sealed record Request([FromRoute] Guid Id);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete(BaseRoute, Invoke)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithTags(EndpointTag);

    private static async Task<IResult> Invoke(
        [AsParameters] Request request,
        IHandler<Request, Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> handler,
        CancellationToken cancellationToken)
        => await handler.HandleAsync(request, cancellationToken);
}