using Wallet.Application.Dtos.Responses;
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        void Add(WalletAccount wallet);
        Task<WalletDto?> FindByWalletIdProjectionAsync(long userId, Guid walletId);
        Task<WalletAccount?> FindByWalletIdAsync(long userId, Guid walletId);
        Task<WalletAccount?> FindByWalletIdAsync(Guid walletId);
        Task<IReadOnlyList<WalletDto>> FindByUserIdAsync(long userId);
        void Update(WalletAccount wallet);
    }
}
