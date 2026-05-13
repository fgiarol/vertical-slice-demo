using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Steps.ChangeStep;

using static Endpoint;

public sealed class Handler(
    ITodoRepository todoRepository,
    IUnitOfWork unitOfWork)
    : IHandler<Request, Results<Ok<Response>, ValidationProblem, NotFound<ProblemDetails>, InternalServerError<ProblemDetails>>>
{
    public async Task<Results<
        Ok<Response>, 
        ValidationProblem, 
        NotFound<ProblemDetails>, 
        InternalServerError<ProblemDetails>>> HandleAsync(Request request, CancellationToken cancellationToken)
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

        step.Update(request.Body.Title, request.Body.IsCompleted, request.Body.Order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new Response(
            step.Id,
            step.Title,
            step.IsCompleted,
            step.Order);

        return TypedResults.Ok(response);
    }
}