using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Todos.ChangeTodo;

using static Endpoint;

public sealed class Handler(
    ITodoRepository todoRepository,
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork)
    : IHandler<Request, Results<Ok<Response>, ValidationProblem, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Ok<Response>, ValidationProblem, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (todo is null)
            return TypedResults.NotFound(new ProblemDetails 
            { 
                Detail = $"Todo with ID {request.Id} was not found", 
                Status = StatusCodes.Status404NotFound,
                Title = "Todo Not Found" 
            });
        
        if (!todo.Steps.All(s => s.IsCompleted) && request.Body.IsCompleted)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "IsCompleted", ["Todo cannot be completed if there are incomplete steps"] }
            });

        var tags = await GetTagsAsync(request.Body.TagIds, cancellationToken);

        todo.Update(
            request.Body.Title,
            request.Body.Description,
            request.Body.Type,
            request.Body.IsCompleted,
            tags);

        await todoRepository.Update(todo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new Response(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Type,
            todo.Steps.Select(s => new Response.Step(s.Id, s.Title, s.IsCompleted, s.Order)),
            todo.Tags.Select(t => new Response.Tag(t.Id, t.Name)));

        return TypedResults.Ok(response);
    }

    private async Task<List<Tag>?> GetTagsAsync(IEnumerable<Guid>? requestTagIds, CancellationToken cancellationToken)
    {
        var ids = requestTagIds?.ToList();
        
        if (ids is null || ids.Count == 0)
            return null;

        var foundTags = await tagRepository.Search(t => ids.Contains(t.Id), cancellationToken);
        return foundTags.ToList();
    }
}