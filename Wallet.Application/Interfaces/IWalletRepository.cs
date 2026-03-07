
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task Add(Domain.Entities.WalletAccount wallet);
        Task<Domain.Entities.WalletAccount?> GetById(Guid id);
        Task Update(Domain.Entities.WalletAccount wallet);
        //Task<bool> WalletExistForUserAsync(Guid userId);
    }
}
