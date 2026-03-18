using Wallet.Application.Common;
using Wallet.Domain.Enums;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Responses;

namespace Wallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletDto>> CreateAsync(long userId, int currency);
        Task<Result<IReadOnlyList<WalletDto>>> GetByUserIdAsync(long userId);
        Task<Result<WalletDto>> GetByWalletIdAsync(Guid walletId);
        Task<Result<WalletDto>> DepositAsync(Guid walletId, DepositRequest request);
        Task<Result<WalletDto>> WithdrawAsync(Guid walletId, WithdrawalRequest request);
        Task<Result<WalletDto>> TransferAsync(Guid walletId, TransferRequest request);
    }
}
