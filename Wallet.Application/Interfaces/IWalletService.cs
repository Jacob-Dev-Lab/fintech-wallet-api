using Wallet.Application.Dtos;
using Wallet.Application.Common;
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletAccount>> CreateWallet();
        Task<Result<WalletAccount>> Deposit(Guid walletID, DepositRequest request);
    }
}
