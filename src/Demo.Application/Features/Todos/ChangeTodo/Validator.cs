using FluentValidation;

namespace Demo.Application.Features.Todos.ChangeTodo;

using static Endpoint;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty");

        RuleFor(r => r.Body.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty");
        
        RuleFor(r => r.Body.Type)
            .IsInEnum();
    }
}