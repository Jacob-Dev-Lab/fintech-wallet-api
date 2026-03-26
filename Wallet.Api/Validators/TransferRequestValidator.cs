using FluentValidation;
using Wallet.Application.Dtos.Requests;

namespace Wallet.Api.Validators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(t => t.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");

            RuleFor(t => t.ReceivingWalletId)
                .NotEmpty()
                .WithMessage("Kindly provide a valid wallet address");

            RuleFor(t => t.Description)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Provide a description for this transaction");
        }
    }
}
