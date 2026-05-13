using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Tags.ChangeTag;

using static Endpoint;

public sealed class Handler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    : IHandler<Request, Results<Ok<Response>, ValidationProblem, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Ok<Response>, ValidationProblem, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.FindById(request.Id, cancellationToken);

        if (tag is null)
            return TypedResults.NotFound(new ProblemDetails 
            { 
                Detail = $"Tag with ID {request.Id} was not found", 
                Status = StatusCodes.Status404NotFound,
                Title = "Tag Not Found" 
            });

        var alreadyExists = await tagRepository.GetTagByName(request.Body.Name, cancellationToken);
        
        if (alreadyExists is not null && alreadyExists.Id != request.Id)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Tag", ["This tag name already exists"] }
            });

        tag.ChangeName(request.Body.Name);

        await tagRepository.Update(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(new Response(tag.Id, tag.Name));
    }
}