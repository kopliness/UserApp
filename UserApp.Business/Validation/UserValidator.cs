using BusinessLayer.DTO;
using FluentValidation;

namespace BusinessLayer.Validation;

public class UserValidator : AbstractValidator<UserCreateDto>
{
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(user => user.Age)
            .NotEmpty().WithMessage("Age is required.")
            .InclusiveBetween(1, 100).WithMessage("Age must be between 1 and 100.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(user => user.Roles)
            .NotEmpty().WithMessage("Roles are required.")
            .Must(roles => roles.All(role => role > 0)).WithMessage("All role IDs must be positive integers.");
    }
}