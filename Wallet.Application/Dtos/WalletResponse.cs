using Wallet.Domain.Enums;

namespace Wallet.Application.Dtos
{
    public class WalletResponse
    {
        public Guid WalletId { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
    }
}
