using FluentValidation;

namespace Demo.Application.Features.Todos.CreateTodo;

using static Endpoint;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty");

        RuleFor(r => r.Steps)
            .NotEmpty()
            .WithMessage("At least one step is required")
            .Must(HasUniqueOrderValues)
            .WithMessage("Steps must have unique order values");

        RuleForEach(r => r.Steps)
            .ChildRules(step => 
            { 
                step.RuleFor(s => s.Title)
                    .NotEmpty()
                    .WithMessage("Step title cannot be empty");
            });
    }
    
    private static bool HasUniqueOrderValues(IEnumerable<Request.Step> steps)
    {
        var list = steps.ToList();
        return list.Select(s => s.Order).Distinct().Count() == list.Count;
    }
}