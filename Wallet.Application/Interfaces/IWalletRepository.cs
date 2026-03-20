using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        void Add(WalletAccount wallet);
        Task<WalletAccount?> FindByWalletIdAsync(Guid walletId);
        IQueryable<WalletAccount> FindByUserId(long userId);
        void Update(WalletAccount wallet);
    }
}
