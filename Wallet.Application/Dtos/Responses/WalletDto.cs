using Wallet.Domain.Enums;

namespace Wallet.Application.Dtos.Responses
{
    public class WalletDto
    {
        public Guid WalletId { get; set; }
        public string? Currency { get; set; }
        public decimal Balance { get; set; }
    }
}
