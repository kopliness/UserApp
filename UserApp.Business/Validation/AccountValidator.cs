using BusinessLayer.DTO;
using FluentValidation;

namespace BusinessLayer.Validation;

public class AccountValidator : AbstractValidator<AccountDto>
{
    public AccountValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login should not be blank")
            .MinimumLength(6).WithMessage("Login must be a minimum of 6 characters")
            .MaximumLength(24).WithMessage("Login must be a maximum of 24 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password should not be blank")
            .MinimumLength(8).WithMessage("Password must be a minimum of 8 characters")
            .MaximumLength(24).WithMessage("The password must be a maximum of 24 characters")
            .Matches("[A-Z]").WithMessage("The password must contain capital letters")
            .Matches("[a-z]").WithMessage("The password must contain lowercase letters")
            .Matches("[0-9]").WithMessage("The password must contain digits");
    }
}