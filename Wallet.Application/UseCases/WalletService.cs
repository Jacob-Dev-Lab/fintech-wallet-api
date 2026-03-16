using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Dtos;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Domain.Enums;
using Wallet.Domain.Exceptions;

namespace Wallet.Application.UseCases
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IGlobalDbOperation _dbOperation;

        public WalletService(IWalletRepository walletRepository, 
            ITransactionRepository transactionRepository,
            IGlobalDbOperation dbOperation)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _dbOperation = dbOperation;
        }

        public async Task<Result<WalletResponse>> CreateAsync(long userId, int currency)
        {
            try
            {
                var currencyType = (Currency) currency;

                var wallet = new WalletAccount(userId, currencyType);

                await _walletRepository.AddAsync(wallet);
                await _dbOperation.SaveChangesAsync();

                var response = new WalletResponse
                {
                    WalletId = wallet.WalletId,
                    Currency = currencyType,
                    Balance = wallet.Balance
                };

                return Result<WalletResponse>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletResponse>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<WalletResponse>>> GetByUserIdAsync(long userId)
        {
            try
            {
                var wallets = await _walletRepository.GetByUserId(userId)
                    .Select( w => new WalletResponse
                    {
                        WalletId = w.WalletId,
                        Currency = w.Currency,
                        Balance = w.Balance
                    }).ToListAsync();

                return Result<IReadOnlyList<WalletResponse>>.Success(wallets);
            }
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<WalletResponse>>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletResponse>> GetByWalletIdAsync(Guid walletId)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Require valid wallet id.");

            try
            {
                var wallet = await _walletRepository.GetByWalletIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletResponse>.Failure("Wallet not found");

                var response = new WalletResponse
                {
                    Balance = wallet.Balance,
                    Currency = (Currency)wallet.Currency
                };

                return Result<WalletResponse>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletResponse>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletResponse>> DepositAsync(Guid walletId, DepositRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Require valid wallet id.");

            try
            {
                var wallet = await _walletRepository.GetByWalletIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletResponse>.Failure("Wallet not found.");

                wallet.Deposit(request.Amount);

                var transaction = new Transaction(
                    userId: request.UserId,
                    walletId: walletId,
                    type: TransactionType.Deposit,
                    amount: request.Amount,
                    balance: wallet.Balance,
                    description: request.Description
                    );

                await _walletRepository.UpdateAsync(wallet);
                await _transactionRepository.AddAsync(transaction);
                await _dbOperation.SaveChangesAsync();

                var response = new WalletResponse
                {
                    WalletId = wallet.WalletId,
                    Currency = wallet.Currency,
                    Balance = wallet.Balance
                };

                return Result<WalletResponse>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletResponse>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletResponse>> WithdrawAsync(Guid walletId, WithdrawalRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Require valid wallet id.");

            try
            {
                var wallet = await _walletRepository.GetByWalletIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletResponse>.Failure("Wallet not found");

                wallet.Withdraw(request.Amount);

                var transaction = new Transaction(
                    userId: request.UserId,
                    walletId: walletId,
                    type: TransactionType.Withdrawal,
                    amount: request.Amount,
                    balance: wallet.Balance,
                    description: request.Description
                    );

                await _walletRepository.UpdateAsync(wallet);
                await _transactionRepository.AddAsync(transaction);
                await _dbOperation.SaveChangesAsync();

                var response = new WalletResponse
                {
                    WalletId = wallet.WalletId,
                    Balance = wallet.Balance
                };

                return Result<WalletResponse>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletResponse>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletResponse>> TransferAsync(Guid walletId, TransferRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Invalid wallet id, try again.");

            if (walletId == request.ReceivingWalletId)
                return Result<WalletResponse>.Failure("Sending and receiving wallet cannot be the same");

            try
            {
                var sendingWallet = await _walletRepository.GetByWalletIdAsync(walletId);
                var receivingWallet = await _walletRepository.GetByWalletIdAsync(request.ReceivingWalletId);

                if (sendingWallet is null || receivingWallet is null)
                    return Result<WalletResponse>.Failure("Either of the wallet is invalid");

                var senderWalletId = walletId;
                var receiverWalletId = request.ReceivingWalletId;

                sendingWallet.Withdraw(request.Amount);
                receivingWallet.Deposit(request.Amount);

                var senderTransaction = new Transaction(
                    userId: sendingWallet.UserId,
                    walletId: senderWalletId,
                    type: TransactionType.TransferOut,
                    amount: request.Amount,
                    balance: sendingWallet.Balance,
                    description: $"Transfer to {receiverWalletId}: {request.Description}",
                    referenceWalletId: receiverWalletId
                    );

                var receiverTransaction = new Transaction(
                    userId: receivingWallet.UserId,
                    walletId: receiverWalletId,
                    type: TransactionType.TransferIn,
                    amount: request.Amount,
                    balance: receivingWallet.Balance,
                    description: $"Transfer from {senderWalletId}: {request.Description}",
                    referenceWalletId: senderWalletId
                    );

                await _walletRepository.UpdateAsync(sendingWallet);
                await _walletRepository.UpdateAsync(receivingWallet);
                await _transactionRepository.AddAsync(senderTransaction);
                await _transactionRepository.AddAsync(receiverTransaction);
                await _dbOperation.SaveChangesAsync();

                var response = new WalletResponse
                {
                    WalletId = senderWalletId,
                    Currency = sendingWallet.Currency,
                    Balance = sendingWallet.Balance

                };

                return Result<WalletResponse>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletResponse>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
