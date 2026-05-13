using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Todos.CreateTodo;

using static Endpoint;

public sealed class Handler(
    ITodoRepository todoRepository,
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork)
    : IHandler<Request, Results<Created<Response>, ValidationProblem, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<Created<Response>, ValidationProblem, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var steps = request.Steps
            .Select(s => new Step(s.Title, false, s.Order))
            .ToList();

        var tags = await GetTagsAsync(request.TagIds, cancellationToken);

        var todo = new Todo(
            request.Title,
            steps,
            request.Type,
            request.Description,
            tags);

        todoRepository.Add(todo);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new Response(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Type,
            todo.Steps.Select(s => new Response.Step(s.Id, s.Title, s.IsCompleted, s.Order)),
            todo.Tags.Select(t => new Response.Tag(t.Id, t.Name)));

        return TypedResults.Created($"/{BaseRoute}/{todo.Id}", response);
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