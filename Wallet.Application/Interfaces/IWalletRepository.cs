using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task AddAsync(WalletAccount wallet);
        Task<WalletAccount?> GetByWalletIdAsync(Guid walletId);
        IQueryable<WalletAccount> GetByUserId(long userId);
    }
}
