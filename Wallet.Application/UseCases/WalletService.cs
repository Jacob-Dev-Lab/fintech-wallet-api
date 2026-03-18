using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Responses;
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

        public async Task<Result<WalletDto>> CreateAsync(long userId, int currency)
        {
            try
            {
                var currencyType = (Currency) currency;

                var wallet = new WalletAccount(userId, currencyType);

                await _walletRepository.AddAsync(wallet);
                await _dbOperation.SaveChangesAsync();

                var response = new WalletDto
                {
                    WalletId = wallet.WalletId,
                    Currency = wallet.Currency.ToString(),
                    Balance = wallet.Balance
                };

                return Result<WalletDto>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<WalletDto>>> GetByUserIdAsync(long userId)
        {
            try
            {
                var wallets = await _walletRepository.GetByUserId(userId)
                    .Select( w => new WalletDto
                    {
                        WalletId = w.WalletId,
                        Currency = w.Currency.ToString(),
                        Balance = w.Balance
                    }).ToListAsync();

                return Result<IReadOnlyList<WalletDto>>.Success(wallets);
            }
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<WalletDto>>.Failure(new Error(ErrorType.NotFound, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletDto>> GetByWalletIdAsync(Guid walletId)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));

            try
            {
                var wallet = await _walletRepository.GetByWalletIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found"));

                var response = new WalletDto
                {
                    WalletId = wallet.WalletId,
                    Balance = wallet.Balance,
                    Currency = wallet.Currency.ToString()
                };

                return Result<WalletDto>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletDto>> DepositAsync(Guid walletId, DepositRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet."));

            if (request.Amount <= 0)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));

            try
            {
                var wallet = await _walletRepository.GetByWalletIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found."));

                await _dbOperation.ExecuteTransactionAsync(async () =>
                {
                    wallet.Deposit(request.Amount);

                    var transaction = Transaction.CreateDeposit(
                        userId: request.UserId,
                        walletId: walletId,
                        amount: request.Amount,
                        balance: wallet.Balance,
                        description: request.Description
                        );

                    await _transactionRepository.AddAsync(transaction);
                });

                var response = new WalletDto
                {
                    WalletId = wallet.WalletId,
                    Currency = wallet.Currency.ToString(),
                    Balance = wallet.Balance
                };  
                    
                return Result<WalletDto>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletDto>> WithdrawAsync(Guid walletId, WithdrawalRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));

            if (request.Amount <= 0)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));

            try
            {
                var wallet = await _walletRepository.GetByWalletIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found"));

                await _dbOperation.ExecuteTransactionAsync(async () =>
                {
                    wallet.Withdraw(request.Amount);

                    var transaction = Transaction.CreateWithdrawal(
                    userId: request.UserId,
                    walletId: walletId,
                    amount: request.Amount,
                    balance: wallet.Balance,
                    description: request.Description
                    );

                    await _transactionRepository.AddAsync(transaction);
                });

                var response = new WalletDto
                {
                    WalletId = wallet.WalletId,
                    Balance = wallet.Balance
                };

                return Result<WalletDto>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<WalletDto>> TransferAsync(Guid walletId, TransferRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id."));

            if (request.Amount <= 0)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));

            if (walletId == request.ReceivingWalletId)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Cannot transfer to self."));

            try
            {
                var sender = await _walletRepository.GetByWalletIdAsync(walletId);
                var receiver = await _walletRepository.GetByWalletIdAsync(request.ReceivingWalletId);

                if (sender is null || receiver is null)
                    return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Either of the wallet is invalid"));

                var senderWalletId = walletId;
                var receiverWalletId = request.ReceivingWalletId;

                await _dbOperation.ExecuteTransactionAsync(async () =>
                {
                    sender.TransferTo(receiver, request.Amount);

                    var senderTransaction = Transaction.CreateTransferOut(
                        sender.UserId,
                        walletId,
                        request.Amount,
                        sender.Balance,
                        receiverWalletId,
                        request.Description
                        );

                    var receiverTransaction = Transaction.CreateTransferIn(
                        receiver.UserId,
                        receiverWalletId,
                        request.Amount,
                        receiver.Balance,
                        senderWalletId,
                        request.Description
                        );

                    await _transactionRepository.AddAsync(senderTransaction);
                    await _transactionRepository.AddAsync(receiverTransaction);
                });

                var response = new WalletDto
                {
                    WalletId = senderWalletId,
                    Currency = sender.Currency.ToString(),
                    Balance = sender.Balance

                };

                return Result<WalletDto>.Success(response);
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }
    }
}
