using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Dtos;
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

        public async Task<Result<IReadOnlyList<TransactionResponse>>> GetByUserIdAsync(long userId)
        {
            try
            {
                var transactions = await _transactionRepository.GetByUserIdAsync(userId)
                    .Where(t => t.UserId == userId)
                    .Select(t => new TransactionResponse
                    {
                        DateCreated = t.CreatedAt,
                        Transaction = t.Type.ToString(),
                        Amount = t.Amount,
                        Description = t.Description,
                        Balance = t.Balance
                    })
                    .ToListAsync();

                return Result<IReadOnlyList<TransactionResponse>>.Success(transactions);
            }
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<TransactionResponse>>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<TransactionResponse>>> GetByWalletIdAsync(Guid walletId)
        {
            try
            {
                var transactions = await _transactionRepository.GetByWalletIdAsync(walletId)
                    .Where(t => t.WalletId == walletId)
                    .Select(t => new TransactionResponse
                    {
                        DateCreated = t.CreatedAt,
                        Transaction = t.Type.ToString(),
                        Amount = t.Amount,
                        Description = t.Description,
                        Balance = t.Balance
                    })
                    .ToListAsync();

                return Result<IReadOnlyList<TransactionResponse>>.Success(transactions);
            }
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<TransactionResponse>>.Failure(ex.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
