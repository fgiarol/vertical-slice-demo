using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Steps.ChangeStep;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Steps";
    private const string BaseRoute = "todos";
    private const string ContentType = "application/json";

    public sealed record Request([FromRoute] Guid TodoId, [FromRoute] Guid StepId, [FromBody] Request.Step Body)
    {
        public sealed record Step(string Title, int Order, bool IsCompleted);
    }

    public sealed record Response(
        Guid Id,
        string Title,
        bool IsCompleted,
        int Order);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut($"{BaseRoute}/{{todoId:guid}}/steps/{{stepId:guid}}", Invoke)
            .Accepts<Request.Step>(ContentType)
            .AddValidationFilter<Request>()
            .Produces<Response>(StatusCodes.Status200OK, ContentType)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithTags(EndpointTag);

    private static async Task<IResult> Invoke(
        [AsParameters] Request request,
        IHandler<Request, Results<
            Ok<Response>, 
            ValidationProblem, 
            NotFound<ProblemDetails>, 
            InternalServerError<ProblemDetails>>> handler,
        CancellationToken cancellationToken)
        => await handler.HandleAsync(request, cancellationToken);
}