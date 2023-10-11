using FluentValidation;
using UserApp.Common;

namespace BusinessLayer.Validation;

public class UserParametersValidator : AbstractValidator<UserParameters>
{
    public UserParametersValidator()
    {
        RuleFor(user => user.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1");

        RuleFor(user => user.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1")
            .LessThanOrEqualTo(50).WithMessage($"PageSize cannot be more than {50}");

        RuleFor(user => user.OrderBy)
            .Must(value =>
                value == null || value.ToLower() == "name" || value.ToLower()  == "age" || value.ToLower()  == "email" || value.ToLower()  == "roleName")
            .WithMessage("OrderBy can only be 'name', 'age', 'email', or 'roleName'");

        RuleFor(user => user.Name)
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("Name can only contain letters and spaces");

        RuleFor(user => user.AgeFrom)
            .GreaterThanOrEqualTo(0).WithMessage("AgeFrom must be greater than or equal to 0");

        RuleFor(user => user.AgeTo)
            .GreaterThanOrEqualTo(0).WithMessage("AgeTo must be greater than or equal to 0");

        RuleFor(user => user.Email)
            .Matches(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$").WithMessage("Email is not valid");

        RuleFor(user => user.RoleName)
            .Matches(@"^[a-zA-Z\s]*$").WithMessage("RoleName can only contain letters and spaces")
            .Must(value => value == null || value.ToLower() == "user" || value.ToLower()  == "support" || value.ToLower()  == "admin" ||
                           value.ToLower()  == "superadmin")
            .WithMessage("RoleName can only be 'user', 'support', 'admin', or 'superadmin'");
    }
}