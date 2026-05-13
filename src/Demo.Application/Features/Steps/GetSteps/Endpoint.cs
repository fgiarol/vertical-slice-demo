using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Steps.GetSteps;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Steps";
    private const string BaseRoute = "todos";
    private const string ContentType = "application/json";

    public sealed record Request([FromRoute] Guid TodoId);

    public sealed record Response(
        Guid Id,
        string Title,
        bool IsCompleted,
        int Order);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet($"{BaseRoute}/{{todoId:guid}}/steps", Invoke)
            .Produces<IEnumerable<Response>>(StatusCodes.Status200OK, ContentType)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithTags(EndpointTag);

    private static async Task<IResult> Invoke(
        [AsParameters] Request request,
        IHandler<Request, Results<Ok<IEnumerable<Response>>, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> handler,
        CancellationToken cancellationToken)
        => await handler.HandleAsync(request, cancellationToken);
}