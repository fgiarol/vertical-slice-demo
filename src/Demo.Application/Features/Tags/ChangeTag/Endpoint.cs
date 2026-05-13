using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Tags.ChangeTag;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Tags";
    private const string BaseRoute = "tags/{id:guid}";
    private const string ContentType = "application/json";

    public sealed record Request([FromRoute] Guid Id, [FromBody] Request.Tag Body)
    {
        public sealed record Tag(string Name);
    }
    
    public sealed record Response(Guid Id, string Name);

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut(BaseRoute, Invoke)
            .Accepts<Request.Tag>(ContentType)
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