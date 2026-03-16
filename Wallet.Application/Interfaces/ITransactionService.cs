using Wallet.Application.Common;
using Wallet.Application.Dtos;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Result<IReadOnlyList<TransactionResponse>>> GetByWalletIdAsync(Guid walletId);
        Task<Result<IReadOnlyList<TransactionResponse>>> GetByUserIdAsync(long userISd);
    }
}
