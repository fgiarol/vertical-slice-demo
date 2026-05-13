using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Steps.DeleteStep;

using static Endpoint;

public sealed class Handler(
    ITodoRepository todoRepository,
    IStepRepository stepRepository,
    IUnitOfWork unitOfWork)
    : IHandler<Request, Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<NoContent, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>> HandleAsync(
        Request request, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Todo not found",
                Detail = $"Todo with id {request.TodoId} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var step = todo.Steps.FirstOrDefault(s => s.Id == request.StepId);

        if (step is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Step not found",
                Detail = $"Step with id {request.StepId} was not found in Todo {request.TodoId}.",
                Status = StatusCodes.Status404NotFound
            });
        }

        await stepRepository.Remove(step, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}