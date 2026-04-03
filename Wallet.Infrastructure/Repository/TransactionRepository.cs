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

        public void Add(Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            _dbContext.Transactions.Add(transaction);
        }

        public async Task<Transaction?> FindByIdAsync(Guid transactionId)
        {
            return await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<Transaction?> FindByWalletIdAsync(Guid walletId)
        {
            return await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.WalletId == walletId);
        }

        public IQueryable<Transaction> FindByUserId(long userId)
        {
            return _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);
        }
    }
}
