using FluentValidation;
using Wallet.Application.Dtos.Requests;

namespace Wallet.Api.Validators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(t => t.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");
        }
    }
}
