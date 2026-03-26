using FluentValidation;
using Wallet.Application.Dtos.Requests;

namespace Wallet.Api.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty()
                .WithMessage("A valid name is required")
                .MaximumLength(100);

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required")
                .MaximumLength(100);

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage("Please provide ypur password")
                .MaximumLength(30);

            RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Kindly comfirm password")
                .Equal(u => u.Password)
                .WithMessage("Password mis-match");
        }
    }
}
