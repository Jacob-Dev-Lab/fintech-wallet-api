using Wallet.Application.Dtos.Responses;
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionRepository
    {
        void Add(Transaction transaction);
        Task<TransactionDto?> FindByIdAsync(long userId, Guid transactionId);
        Task<IReadOnlyList<TransactionDto>> FindByWalletIdAsync(long userId, Guid walletId);
        Task<IReadOnlyList<TransactionDto>> FindByUserIdAsync(long userId);
    }
}


