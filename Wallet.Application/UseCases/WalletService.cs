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

        public WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<WalletResponse>> CreateAsync()
        {
            try
            {
                var wallet = new WalletAccount(Guid.NewGuid(), DateTime.UtcNow);

                await _walletRepository.AddAsync(wallet);

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

        public async Task<Result<IReadOnlyList<WalletResponse>>> GetAllAsync()
        {
            try
            {
                var wallets = await _walletRepository.GetAllAsync();

                var responses = wallets.Select(
                    w => new WalletResponse
                    {
                        WalletId = w.WalletId,
                        Balance = w.Balance
                    })
                    .ToList();

                return Result<IReadOnlyList<WalletResponse>>.Success(responses);
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

        public async Task<Result<WalletResponse>> GetByIdAsync(Guid walletId)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Invalid wallet id, try again.");

            try
            {
                var wallet = await _walletRepository.GetByIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletResponse>.Failure("Invalid walletId");

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

        public async Task<Result<WalletResponse>> DepositAsync(Guid walletId, DepositRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Invalid wallet id, try again.");

            try
            {
                var wallet = await _walletRepository.GetByIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletResponse>.Failure("Invalid wallet id.");

                wallet.Deposit(request.Amount);

                var transaction = new Transaction(
                    walletId: walletId,
                    type: TransactionType.Deposit,
                    amount: request.Amount,
                    balance: wallet.Balance,
                    description: request.Description
                    );

                await _transactionRepository.AddAsync(transaction);
                await _walletRepository.UpdateAsync(wallet);

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

        public async Task<Result<WalletResponse>> WithdrawAsync(Guid walletId, WithdrawalRequest request)
        {
            if (walletId == Guid.Empty)
                return Result<WalletResponse>.Failure("Invalid wallet id, try again.");

            try
            {
                var wallet = await _walletRepository.GetByIdAsync(walletId);

                if (wallet is null)
                    return Result<WalletResponse>.Failure("Invalid wallet id.");

                wallet.Withdraw(request.Amount);

                var transaction = new Transaction(
                    walletId: walletId,
                    type: TransactionType.Withdrawal,
                    amount: request.Amount,
                    balance: wallet.Balance,
                    description: request.Description
                    );

                await _transactionRepository.AddAsync(transaction);
                await _walletRepository.UpdateAsync(wallet);

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
                var sendingWallet = await _walletRepository.GetByIdAsync(walletId);
                var receivingWallet = await _walletRepository.GetByIdAsync(request.ReceivingWalletId);

                if (sendingWallet is null || receivingWallet is null)
                    return Result<WalletResponse>.Failure("Either of the wallet is invalid");

                var sender = walletId;
                var receiver = request.ReceivingWalletId;

                sendingWallet.Withdraw(request.Amount);
                receivingWallet.Deposit(request.Amount);

                var senderTransaction = new Transaction(
                    walletId: sender,
                    type: TransactionType.TransferTo,
                    amount: request.Amount,
                    balance: sendingWallet.Balance,
                    description: $"Transfer to {receiver}: {request.Description}",
                    referenceWalletId: receiver
                    );

                var receiverTransaction = new Transaction(
                    walletId: receiver,
                    type: TransactionType.TransferFrom,
                    amount: request.Amount,
                    balance: receivingWallet.Balance,
                    description: $"Transfer from {sender}: {request.Description}",
                    referenceWalletId: sender
                    );

                await _walletRepository.UpdateAsync(sendingWallet);
                await _transactionRepository.AddAsync(senderTransaction);

                await _walletRepository.UpdateAsync(receivingWallet);
                await _transactionRepository.AddAsync(receiverTransaction);

                var response = new WalletResponse
                {
                    WalletId = sender,
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
