using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Tags.GetTags;

using static Endpoint;

public sealed class Handler(ITagRepository tagRepository) 
    : IHandler<Request, Results<Ok<IEnumerable<Response>>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Ok<IEnumerable<Response>>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.GetAllTags(cancellationToken);
        
        var result = tags.Select(tag => new Response(tag.Id, tag.Name));
        
        return TypedResults.Ok(result);
    }
}