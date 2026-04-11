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

        public async Task<Result<IReadOnlyList<TransactionDto>>> GetByUserIdAsync(long userId)
        {
            var transactions = await _transactionRepository.FindByUserId(userId)
                .Select(t => new TransactionDto
                {
                    DateCreated = t.CreatedAt,
                    Transaction = t.Type.ToString(),
                    Amount = t.Amount,
                    Description = t.Description,
                    Balance = t.Balance
                })
                .ToListAsync();

            return Result<IReadOnlyList<TransactionDto>>.Success(transactions);
        }

        public async Task<Result<IReadOnlyList<TransactionDto>>> GetByWalletIdAsync(long userId, Guid walletId)
        {
            if (walletId == Guid.Empty)
                return Result<IReadOnlyList<TransactionDto>>
                    .Failure(new Error(ErrorType.BadRequest, ["Invalid wallet id."]));

            var transactions = await _transactionRepository.FindByUserId(userId)
                .Where(t =>  t.WalletId == walletId)
                .Select(t => new TransactionDto
                {
                    DateCreated = t.CreatedAt,
                    Transaction = t.Type.ToString(),
                    Amount = t.Amount,
                    Description = t.Description,
                    Balance = t.Balance
                })
                .ToListAsync();

            return Result<IReadOnlyList<TransactionDto>>.Success(transactions);
        }
    }
}
