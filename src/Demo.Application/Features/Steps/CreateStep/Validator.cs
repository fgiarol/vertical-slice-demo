using FluentValidation;

namespace Demo.Application.Features.Steps.CreateStep;

using static Endpoint;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.TodoId)
            .NotEmpty()
            .WithMessage("TodoId is required");

        RuleFor(r => r.Body.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty");

        RuleFor(r => r.Body.Order)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Order must be greater than or equal to 1");
    }
}