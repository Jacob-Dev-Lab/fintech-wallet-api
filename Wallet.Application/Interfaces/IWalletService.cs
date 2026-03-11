using Wallet.Application.Dtos;
using Wallet.Application.Common;

namespace Wallet.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Result<WalletResponse>> CreateAsync();
        Task<Result<IReadOnlyList<WalletResponse>>> GetAllAsync();
        Task<Result<WalletResponse>> GetByIdAsync(Guid walletId);
        Task<Result<WalletResponse>> DepositAsync(Guid walletId, DepositRequest request);
        Task<Result<WalletResponse>> WithdrawAsync(Guid walletId, WithdrawalRequest request);
        Task<Result<WalletResponse>> TransferAsync(Guid walletId, TransferRequest request);
    }
}
