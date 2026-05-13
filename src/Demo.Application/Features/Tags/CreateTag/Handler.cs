using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Tags.CreateTag;

using static Endpoint;

public sealed class Handler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    : IHandler<Request, Results<Created<Response>, ValidationProblem, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Created<Response>, ValidationProblem, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var alreadyExists = await tagRepository.GetTagByName(request.Name, cancellationToken) is not null;

        if (alreadyExists)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Tag", ["This tag already exists"] }
            });

        var tag = new Tag(request.Name);

        tagRepository.Add(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/{BaseRoute}/{tag.Id}", new Response(tag.Id, request.Name));
    }
}