using Wallet.Application.Common;
using Wallet.Application.Dtos.Responses;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Result<IReadOnlyList<TransactionDto>>> GetByWalletIdAsync(long userId, Guid walletId);
        Task<Result<IReadOnlyList<TransactionDto>>> GetByUserIdAsync(long userISd);
    }
}
