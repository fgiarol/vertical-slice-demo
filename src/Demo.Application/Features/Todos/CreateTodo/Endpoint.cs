using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Demo.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Todos.CreateTodo;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Todos";
    internal const string BaseRoute = "todos";
    private const string ContentType = "application/json";

    public sealed record Request(
        string Title,
        string? Description,
        TodoType Type,
        IEnumerable<Request.Step> Steps,
        IEnumerable<Guid>? TagIds)
    {
        public sealed record Step(string Title, int Order);
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
        app.MapPost(BaseRoute, Invoke)
            .Accepts<Request>(ContentType)
            .AddValidationFilter<Request>()
            .Produces<Response>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithTags(EndpointTag);

    private static async Task<IResult> Invoke(
        Request request,
        IHandler<Request, Results<Created<Response>, ValidationProblem, InternalServerError<ProblemDetails>>> handler,
        CancellationToken cancellationToken)
        => await handler.HandleAsync(request, cancellationToken);
}