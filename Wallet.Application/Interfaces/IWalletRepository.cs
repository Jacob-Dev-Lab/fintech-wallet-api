
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task AddAsync(WalletAccount wallet);
        Task<WalletAccount?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<WalletAccount>> GetAllAsync();
        Task UpdateAsync(WalletAccount wallet);
        //Task DeleteAsync(Guid id);
        //Task<bool> WalletExistForUserAsync(Guid userId);
    }
}
