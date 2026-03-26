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
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IWalletRepository walletRepository, 
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<WalletDto>> CreateAsync(long userId, int currency)
        {
            try
            {
                var currencyType = (Currency) currency;

                var wallet = new WalletAccount(userId, currencyType);

                _walletRepository.Add(wallet);
                await _unitOfWork.SaveChangesAsync();

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
        }

        public async Task<Result<IReadOnlyList<WalletDto>>> GetByUserIdAsync(long userId)
        {
            var wallets = await _walletRepository
                .FindByUserId(userId)
                .Select( w => new WalletDto
                {
                    WalletId = w.WalletId,
                    Currency = w.Currency.ToString(),
                    Balance = w.Balance
                }).ToListAsync();

            return Result<IReadOnlyList<WalletDto>>.Success(wallets);
        }

        public async Task<Result<WalletDto>> GetByWalletIdAsync(long userId, Guid walletId)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));

            var wallet = await _walletRepository
                .FindByUserId(userId)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);

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

        public async Task<Result<WalletDto>> DepositAsync(long userId, Guid walletId, DepositRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet."));

            if (request.Amount <= 0)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));

            var wallet = await _walletRepository
                .FindByUserId(userId)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);

            if (wallet is null)
                return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found."));

            try
            {
                await _unitOfWork.ExecuteTransactionAsync(async () =>
                {
                    wallet.Deposit(request.Amount);

                    var transaction = Transaction.CreateDeposit(
                        userId: userId,
                        walletId: walletId,
                        amount: request.Amount,
                        balance: wallet.Balance,
                        description: request.Description
                        );

                    _walletRepository.Update(wallet);
                    _transactionRepository.Add(transaction);
                    await Task.CompletedTask;
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<WalletDto>.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }

            var response = new WalletDto
            {
                WalletId = wallet.WalletId,
                Currency = wallet.Currency.ToString(),
                Balance = wallet.Balance
            };  
                    
            return Result<WalletDto>.Success(response);
        }

        public async Task<Result<WalletDto>> WithdrawAsync(long userId, Guid walletId, WithdrawalRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));

            if (request.Amount <= 0)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));

            var wallet = await _walletRepository
                .FindByUserId(userId)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);

            if (wallet is null)
                return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found"));

            try
            {
                await _unitOfWork.ExecuteTransactionAsync(async () =>
                {
                    wallet.Withdraw(request.Amount);

                    var transaction = Transaction.CreateWithdrawal(
                    userId: userId,
                    walletId: walletId,
                    amount: request.Amount,
                    balance: wallet.Balance,
                    description: request.Description
                    );

                    _walletRepository.Update(wallet);
                    _transactionRepository.Add(transaction);
                    await Task.CompletedTask;
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<WalletDto>.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }

            var response = new WalletDto
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance
            };

            return Result<WalletDto>.Success(response);
        }

        public async Task<Result<WalletDto>> TransferAsync(long userId, Guid walletId, TransferRequest request)
        {
            if (walletId == Guid.Empty || request.ReceivingWalletId == Guid.Empty)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id."));

            if (request.Amount <= 0)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));

            if (walletId == request.ReceivingWalletId)
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Cannot transfer to self."));
            
            var sender = await _walletRepository
                .FindByUserId(userId)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);

            var receiver = await _walletRepository
                .FindByWalletIdAsync(request.ReceivingWalletId!);

            if (sender is null || receiver is null)
                return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Either of the wallet is invalid"));

            var senderWalletId = walletId;
            var receiverWalletId = request.ReceivingWalletId;

            try
            {
                await _unitOfWork.ExecuteTransactionAsync(async () =>
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

                    _walletRepository.Update(sender);
                    _walletRepository.Update(receiver);
                    _transactionRepository.Add(senderTransaction);
                    _transactionRepository.Add(receiverTransaction);
                    await Task.CompletedTask;
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<WalletDto>.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }

            var response = new WalletDto
            {
                WalletId = senderWalletId,
                Currency = sender.Currency.ToString(),
                Balance = sender.Balance

            };

            return Result<WalletDto>.Success(response);
        }
    }
}
