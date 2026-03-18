using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly WalletApiDbContext _dbContext;

        public TransactionRepository(WalletApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            await _dbContext.Transactions.AddAsync(transaction);
        }

        public async Task<Transaction?> GetByIdAsync(Guid transactionId)
        {
            return await _dbContext.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public IQueryable<Transaction> GetByWalletIdAsync(Guid walletId)
        {
            return _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.WalletId == walletId);
        }

        public IQueryable<Transaction> GetByUserId(long userId)
        {
            return _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);
        }
    }
}
