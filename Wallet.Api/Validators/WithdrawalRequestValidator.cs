using FluentValidation;
using Wallet.Application.Dtos.Requests;

namespace Wallet.Api.Validators
{
    public class WithdrawalRequestValidator : AbstractValidator<WithdrawalRequest>
    {
        public WithdrawalRequestValidator()
        {
            RuleFor(t => t.Amount)
                .GreaterThan(0)
                .WithMessage("Amount mmust be greater than zero");
        }
    }
}
