using System.Data;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Dtos.Responses;
using Wallet.Application.Interfaces;
using Wallet.Domain.Exceptions;

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
            try
            {
                var transactions = await _transactionRepository.GetByUserId(userId)
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
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<TransactionDto>>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<TransactionDto>>> GetByWalletIdAsync(Guid walletId)
        {
            try
            {
                var transactions = await _transactionRepository.GetByWalletIdAsync(walletId)
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
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<TransactionDto>>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }
    }
}
