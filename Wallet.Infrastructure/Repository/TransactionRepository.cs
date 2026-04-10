using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly WalletApiDbContext _dbContext;
        private readonly ILogger<TransactionRepository> _logger;

        public TransactionRepository(WalletApiDbContext dbContext, 
            ILogger<TransactionRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        public void Add(Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            _logger.LogInformation("Adding transaction with ID {TransactionId} for wallet {WalletId}", 
                transaction.TransactionId, transaction.WalletId);

            _dbContext.Transactions.Add(transaction);
        }

        // ---------------------------------------------------------
        // READ-ONLY QUERIES (no tracking)
        // ---------------------------------------------------------
        public async Task<Transaction?> FindByIdAsync(Guid transactionId)
        {
            _logger.LogInformation("Fetching transaction with ID {TransactionId}", transactionId);

            var transaction = await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction is null)
                _logger.LogWarning("Transaction not found. TransactionId={TransactionId}", transactionId);

            return transaction;
        }

        public async Task<Transaction?> FindByWalletIdAsync(Guid walletId)
        {
            _logger.LogInformation("Fetching transaction for wallet ID {WalletId}", walletId);

            var transaction = await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.WalletId == walletId);

            if (transaction is null)
                _logger.LogWarning("Transaction not found. WalletId={WalletId}", walletId);

            return transaction;
        }

        public IQueryable<Transaction> FindByUserId(long userId)
        {
            _logger.LogInformation("Fetching transactions for user ID {UserId}", userId);

            return _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);
        }
    }
}
