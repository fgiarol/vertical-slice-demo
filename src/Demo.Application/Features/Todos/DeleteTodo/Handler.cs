using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Todos.DeleteTodo;

using static Endpoint;

public sealed class Handler(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
    : IHandler<Request, Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.FindById(request.Id, cancellationToken);

        if (todo is null)
            return TypedResults.NotFound(new ProblemDetails
            {
                Detail = $"Todo with ID {request.Id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Todo Not Found"
            });

        await todoRepository.Remove(todo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}