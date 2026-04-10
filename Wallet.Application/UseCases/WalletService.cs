using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<WalletService> _logger;

        public WalletService(IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork, ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<WalletDto>> CreateAsync(long userId, int currency)
        {
            _logger.LogInformation("Creating wallet for user {UserId} with currency {Currency}", userId, currency);

            try
            {
                var currencyType = (Currency)currency;

                var wallet = new WalletAccount(userId, currencyType);

                _walletRepository.Add(wallet);
                await _unitOfWork.SaveChangesAsync();

                var response = new WalletDto
                {
                    WalletId = wallet.WalletId,
                    Currency = wallet.Currency.ToString(),
                    Balance = wallet.Balance
                };

                _logger.LogInformation("Wallet created successfully for user {UserId} with wallet id {WalletId}", 
                    userId, wallet.WalletId);

                return Result<WalletDto>.Success(response);
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Error creating wallet for user {UserId} with currency {Currency}: {ErrorMessage}", 
                    userId, currency, ex.Message);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
        }

        public async Task<Result<IReadOnlyList<WalletDto>>> GetByUserIdAsync(long userId)
        {
            var wallets = await _walletRepository
                .FindByUserId(userId)
                .Select(w => new WalletDto
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

            var wallet = await _walletRepository.FindByWalletIdProjectionAsync(userId, walletId);

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
            _logger.LogInformation("Deposit request recieved. " +
                "UserId = {userId}, WalletId = {walletId}, Amount = {Amount}", 
                userId, walletId, request.Amount);

            if (walletId == Guid.Empty)
            {
                _logger.LogWarning("Deposit request failed due to invalid wallet id. " +
                    "UserId = {userId}, WalletId = {walletId}", 
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet."));
            }

            if (request.Amount <= 0)
            {
                _logger.LogWarning("Deposit request failed due to invalid amount. " +
                    "UserId = {userId}, WalletId = {walletId}, Amount = {Amount}", 
                    userId, walletId, request.Amount);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Amount must be greater than zero."));
            }

            var wallet = await _walletRepository.FindByWalletIdAsync(userId, walletId);

            if (wallet is null)
            {
                _logger.LogWarning("Deposit request failed due to wallet not found. " +
                    "UserId = {userId}, WalletId = {walletId}", 
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found"));
            }

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

                    _logger.LogInformation("Deposit transaction created. " +
                        "UserId={UserId}, WalletId={WalletId}, TransactionId={TransactionId}",
                        userId, walletId, transaction.TransactionId);

                    await Task.CompletedTask;
                });

                _logger.LogInformation("Deposit completed successfully. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency error during deposit. " +
                    "UserId={UserId}, WalletId={WalletId}", 
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain error during deposit. " +
                    "UserId={UserId}, WalletId={WalletId}, ErrorMessage={ErrorMessage}", 
                    userId, walletId, ex.Message);

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
            _logger.LogInformation("Withdrawal request recieved. " +
                "UserId = {userId}, WalletId = {walletId}, Amount = {Amount}",
                userId, walletId, request.Amount);

            if (walletId == Guid.Empty)
            {
                _logger.LogWarning("Withdrawal request failed due to invalid wallet id. " +
                    "UserId = {userId}, WalletId = {walletId}",
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));
            }

            if (request.Amount <= 0)
            {
                _logger.LogWarning("Withdrawal request failed due to invalid amount. " +
                    "UserId = {userId}, WalletId = {walletId}, Amount = {Amount}",
                    userId, walletId, request.Amount);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, 
                    "Amount must be greater than zero."));
            }

            var wallet = await _walletRepository.FindByWalletIdAsync(userId, walletId);

            if (wallet is null)
            {
                _logger.LogWarning("Withdrawal request failed due to wallet not found. " +
                    "UserId = {userId}, WalletId = {walletId}",
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found"));
            }

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

                    _logger.LogInformation("Withdrawal transaction created. " +
                        "UserId={UserId}, WalletId={WalletId}, TransactionId={TransactionId}",
                        userId, walletId, transaction.TransactionId);

                    await Task.CompletedTask;
                });

                _logger.LogInformation("Withdrawal completed successfully. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency error during withdrawal. " +
                    "UserId={UserId}, WalletId={WalletId}",
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain error during withdrawal. " +
                   "UserId={UserId}, WalletId={WalletId}, ErrorMessage={ErrorMessage}",
                   userId, walletId, ex.Message);

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

        public async Task<Result<WalletDto>> TransferAsync(long userId, Guid walletId, TransferRequest request)
        {
            _logger.LogInformation("Transfer request received. UserId={UserId}, " +
                "SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}",
                userId, walletId, request.ReceivingWalletId);

            if (walletId == Guid.Empty || request.ReceivingWalletId == Guid.Empty)
            {
                _logger.LogWarning("Transfer failed: Invalid wallet ID. UserId={UserId}, " +
                    "SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}",
                    userId, walletId, request.ReceivingWalletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id."));
            }

            if (request.Amount <= 0)
            {
                _logger.LogWarning("Transfer failed: Invalid amount. UserId={UserId}," +
                    " SenderWalletId={SenderWalletId}",
                    userId, walletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, 
                    "Amount must be greater than zero."));
            }

            if (walletId == request.ReceivingWalletId)
            {
                _logger.LogWarning("Transfer failed: Attempt to transfer to self. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.BadRequest, 
                    "Cannot transfer to self."));
            }

            var sender = await _walletRepository
                .FindByWalletIdAsync(userId, walletId);

            var receiver = await _walletRepository
                .FindByWalletIdAsync(request.ReceivingWalletId);

            if (sender is null || receiver is null)
            {
                _logger.LogWarning("Transfer failed: Wallet not found. UserId={UserId}, " +
                    "SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}",
                    userId, walletId, request.ReceivingWalletId);

                return Result<WalletDto>.Failure(new Error(ErrorType.NotFound, 
                    "Either of the wallet is invalid"));
            }

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

                    _logger.LogInformation("Transfer transaction created. " +
                        "UserId={UserId}, SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}}",
                        userId, senderWalletId, receiverWalletId);

                    await Task.CompletedTask;
                });

                _logger.LogInformation("Transfer completed successfully. UserId={UserId}, " +
                    "SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}",
                    userId, senderWalletId, receiverWalletId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency error during transfer. UserId={UserId}, " +
                    "SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}",
                    userId, senderWalletId, receiverWalletId);

                return Result<WalletDto>.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain error during transfer. UserId={UserId}, " +
                    "SenderWalletId={SenderWalletId}, ReceiverWalletId={ReceiverWalletId}, " +
                    "ErrorMessage={ErrorMessage}",
                    userId, senderWalletId, receiverWalletId, ex.Message);

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

        public async Task<Result> FreezWalletAsync(long userId, Guid walletId)
        {
            _logger.LogInformation("Freeze wallet request received. UserId={UserId}, " +
                "WalletId={WalletId}", userId, walletId);

            if (walletId == Guid.Empty)
            {
                _logger.LogWarning("Freeze wallet request failed due to invalid wallet id. " +
                    "UserId={UserId}, WalletId={WalletId}", userId, walletId);

                return Result.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));
            }

            var wallet = await _walletRepository.FindByWalletIdAsync(userId, walletId);

            if (wallet is null)
            {
                _logger.LogWarning("Freeze wallet request failed due to wallet not found. " +
                    "UserId={UserId}, WalletId={WalletId}", userId, walletId);

                return Result.Failure(new Error(ErrorType.NotFound, "Wallet not found"));
            }

            try
            {
                wallet.Freeze();
                _walletRepository.Update(wallet);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Wallet frozen successfully. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency error during wallet freeze. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);

                return Result.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain error during wallet freeze. UserId={UserId}, " +
                    "WalletId={WalletId}, ErrorMessage={ErrorMessage}", userId, walletId, ex.Message);

                return Result.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }

            return Result.Success();
        }

        public async Task<Result> UnfreezWalletAsync(long userId, Guid walletId)
        {
            _logger.LogInformation("Unfreeze wallet request received. UserId={UserId}, " +
                "WalletId={WalletId}", userId, walletId);

            if (walletId == Guid.Empty)
            {
                _logger.LogWarning("Unfreeze wallet request failed due to invalid wallet id. " +
                    "UserId={UserId}, WalletId={WalletId}", userId, walletId);

                return Result.Failure(new Error(ErrorType.BadRequest, "Invalid wallet id"));
            }

            var wallet = await _walletRepository.FindByWalletIdAsync(userId, walletId);

            if (wallet is null)
            {
                _logger.LogWarning("Unfreeze wallet request failed due to wallet not found. " +
                    "UserId={UserId}, WalletId={WalletId}", userId, walletId);

                return Result.Failure(new Error(ErrorType.NotFound, "Wallet not found"));
            }

            try
            {
                wallet.UnFreeze();
                _walletRepository.Update(wallet);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Wallet unfrozen successfully. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Concurrency error during wallet unfreeze. UserId={UserId}, " +
                    "WalletId={WalletId}", userId, walletId);

                return Result.Failure(new Error(
                    ErrorType.Conflict, "Error: Multiple update, Try again later."));
            }
            catch (DomainException ex)
            {
                _logger.LogError(ex, "Domain error during wallet unfreeze. UserId={UserId}, " +
                    "WalletId={WalletId}, ErrorMessage={ErrorMessage}", userId, walletId, ex.Message);

                return Result.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }

            return Result.Success();
        }
    }
}
