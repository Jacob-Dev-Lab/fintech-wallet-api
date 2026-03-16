using Wallet.Application.Dtos;
using Wallet.Application.Common;
using Wallet.Domain.Enums;

namespace Wallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletResponse>> CreateAsync(long userId, int currency);
        Task<Result<IReadOnlyList<WalletResponse>>> GetByUserIdAsync(long userId);
        Task<Result<WalletResponse>> GetByWalletIdAsync(Guid walletId);
        Task<Result<WalletResponse>> DepositAsync(Guid walletId, DepositRequest request);
        Task<Result<WalletResponse>> WithdrawAsync(Guid walletId, WithdrawalRequest request);
        Task<Result<WalletResponse>> TransferAsync(Guid walletId, TransferRequest request);
    }
}
