using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Interfaces.Persistence;
using Demo.Application.Common.Interfaces.Persistence.Repositories;
using Demo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Application.Features.Steps.CreateStep;

using static Endpoint;

public sealed class Handler(
    ITodoRepository todoRepository,
    IStepRepository stepRepository,
    IUnitOfWork unitOfWork)
    : IHandler<Request, Results<Created<Response>, ValidationProblem, NotFound<ProblemDetails>,
        InternalServerError<ProblemDetails>>>
{
    public async Task<Results<
        Created<Response>, 
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

        var step = new Step(request.Body.Title, false, request.Body.Order);

        todo.AddSteps([step]);
        stepRepository.Add(step);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new Response(
            step.Id,
            step.Title,
            step.IsCompleted,
            step.Order);

        return TypedResults.Created($"/{BaseRoute}/{todo.Id}/steps/{step.Id}", response);
    }
}