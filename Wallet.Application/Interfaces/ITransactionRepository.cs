using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionRepository
    {
        void Add(Transaction transaction);
        Task<Transaction?> FindByIdAsync(Guid transactionId);
        Task<Transaction?> FindByWalletIdAsync(Guid walletId);
        IQueryable<Transaction> FindByUserId(long userId);
    }
}


