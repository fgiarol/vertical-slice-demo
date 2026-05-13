using FluentValidation;

namespace Demo.Application.Features.Tags.CreateTag;

using static Endpoint;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty");
    }
}