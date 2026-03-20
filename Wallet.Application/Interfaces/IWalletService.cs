using Wallet.Application.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Responses;

namespace Wallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletDto>> CreateAsync(long userId, int currency);
        Task<Result<IReadOnlyList<WalletDto>>> GetByUserIdAsync(long userId);
        Task<Result<WalletDto>> GetByWalletIdAsync(long userId, Guid walletId);
        Task<Result<WalletDto>> DepositAsync(long userId, Guid walletId, DepositRequest request);
        Task<Result<WalletDto>> WithdrawAsync(long userId, Guid walletId, WithdrawalRequest request);
        Task<Result<WalletDto>> TransferAsync(long userId, Guid walletId, TransferRequest request);
    }
}
