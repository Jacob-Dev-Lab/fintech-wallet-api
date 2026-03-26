using FluentValidation;
using Wallet.Application.Dtos.Requests;

namespace Wallet.Api.Validators
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required")
                .MaximumLength(100);

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage("Please provide ypur password");
        }
    }
}
