using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task Add(Transaction transaction)
        {
            if (transaction is null) 
                throw new ArgumentNullException(nameof(transaction), "Ivalid transaction");

            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<Transaction?> GetByWalletId(Guid walletId)
        {
            return await _dbContext.Transactions.FindAsync(walletId);
        }
    }
}
