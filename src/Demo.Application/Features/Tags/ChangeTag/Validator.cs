using FluentValidation;

namespace Demo.Application.Features.Tags.ChangeTag;

using static Endpoint;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty");

        RuleFor(r => r.Body.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty");
    }
}