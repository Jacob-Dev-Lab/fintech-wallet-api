using System.Linq.Expressions;
using Wallet.Domain.Entities;

namespace Wallet.Application.Dtos.Responses
{
    public class WalletDto
    {
        public Guid WalletId { get; set; }
        public string? Currency { get; set; }
        public decimal Balance { get; set; }

        public static Expression<Func<WalletAccount, WalletDto>> Projection =>
            WalletAccount => new WalletDto
            {
                WalletId = WalletAccount.WalletId,
                Currency = WalletAccount.Currency.ToString(),
                Balance = WalletAccount.Balance
             };
}
}
