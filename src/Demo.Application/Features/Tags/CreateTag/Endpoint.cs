using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Demo.Application.Features.Tags.CreateTag;

public sealed class Endpoint : IEndpoint
{
    private const string EndpointTag = "Tags";
    internal const string BaseRoute = "tags";
    private const string ContentType = "application/json";

    public sealed record Request(string Name);
    public sealed record Response(Guid Id, string Name);

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