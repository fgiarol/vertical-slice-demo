using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Steps.CreateStep;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Steps";
    internal const string BaseRoute = "todos";
    private const string ContentType = "application/json";

    public sealed record Request([FromRoute] Guid TodoId, [FromBody] Request.Step Body)
    {
        public sealed record Step(string Title, int Order);
    }

    public sealed record Response(
        Guid Id,
        string Title,
        bool IsCompleted,
        int Order);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost($"{BaseRoute}/{{todoId:guid}}/steps", Invoke)
            .Accepts<Request.Step>(ContentType)
            .AddValidationFilter<Request>()
            .Produces<Response>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithTags(EndpointTag);

    private static async Task<IResult> Invoke(
        [AsParameters] Request request,
        IHandler<Request, Results<
            Created<Response>,
            ValidationProblem,
            NotFound<ProblemDetails>,
            InternalServerError<ProblemDetails>>> handler,
        CancellationToken cancellationToken)
        => await handler.HandleAsync(request, cancellationToken);
}