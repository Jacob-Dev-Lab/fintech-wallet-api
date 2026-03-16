using Wallet.Application.Common;
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(Guid transactionId);
        IQueryable<Transaction> GetByWalletIdAsync(Guid walletId);
        IQueryable<Transaction> GetByUserIdAsync(long userId);
    }
}
