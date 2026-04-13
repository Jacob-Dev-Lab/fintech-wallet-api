using System.Data;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Common.Enum;
using Wallet.Application.Dtos.Responses;
using Wallet.Application.Interfaces;

namespace Wallet.Application.UseCases
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<TransactionDto?>> GetByTransactionIdAsync(long userId, Guid transactionId)
        {
            var transaction = await _transactionRepository.FindByIdAsync(userId, transactionId);

            if (transaction is null)
                return Result<TransactionDto?>.Failure(new Error(ErrorType.NotFound, ["Transaction not found."]));

            return Result<TransactionDto?>.Success(transaction);
        }

        public async Task<Result<IReadOnlyList<TransactionDto>>> GetByWalletIdAsync(long userId, Guid walletId)
        {
            if (walletId == Guid.Empty)
                return Result<IReadOnlyList<TransactionDto>>
                    .Failure(new Error(ErrorType.BadRequest, ["Invalid wallet id."]));

            var transactions = await _transactionRepository.FindByWalletIdAsync(userId, walletId);

            return Result<IReadOnlyList<TransactionDto>>.Success(transactions);
        }

        public async Task<Result<IReadOnlyList<TransactionDto>>> GetByUserIdAsync(long userId)
        {
            var transactions = await _transactionRepository.FindByUserIdAsync(userId);

            return Result<IReadOnlyList<TransactionDto>>.Success(transactions);
        }
    }
}
