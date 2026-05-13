using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Demo.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Todos.ChangeTodo;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Todos";
    private const string BaseRoute = "todos/{id:guid}";
    private const string ContentType = "application/json";

    public sealed record Request([FromRoute] Guid Id, [FromBody] Request.Todo Body)
    {
        public sealed record Todo(
            string Title,
            string? Description,
            TodoType Type,
            bool IsCompleted,
            IEnumerable<Guid>? TagIds);
    }

    public sealed record Response(
        Guid Id,
        string Title,
        string? Description,
        bool IsCompleted,
        TodoType Type,
        IEnumerable<Response.Step> Steps,
        IEnumerable<Response.Tag> Tags)
    {
        public sealed record Step(Guid Id, string Title, bool IsCompleted, int Order);
        public sealed record Tag(Guid Id, string Name);
    }

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut(BaseRoute, Invoke)
            .Accepts<Request.Todo>(ContentType)
            .AddValidationFilter<Request>()
            .Produces<Response>(StatusCodes.Status200OK, ContentType)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithTags(EndpointTag);

    private static async Task<IResult> Invoke(
        [AsParameters] Request request,
        IHandler<Request, Results<Ok<Response>, ValidationProblem, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> handler,
        CancellationToken cancellationToken)
        => await handler.HandleAsync(request, cancellationToken);
}