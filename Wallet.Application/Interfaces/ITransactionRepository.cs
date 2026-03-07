
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task Add(Transaction transaction);
        Task<Transaction?> GetByWalletId(Guid walletId);
    }
}
