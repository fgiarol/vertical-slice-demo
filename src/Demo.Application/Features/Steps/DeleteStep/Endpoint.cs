using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Steps.DeleteStep;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Steps";
    private const string BaseRoute = "todos";

    public sealed record Request([FromRoute] Guid TodoId, [FromRoute] Guid StepId);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete($"{BaseRoute}/{{todoId:guid}}/steps/{{stepId:guid}}", Invoke)
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