using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
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
            await _dbContext.SaveChangesAsync();

        }

        public async Task<Transaction?> GetByIdAsync(Guid Id)
        {
            if (Id == Guid.Empty)
                return null;

            return await _dbContext.Transactions.FindAsync(Id);
        }

        public async Task<IReadOnlyList<Transaction>?> GetByWalletIdAsync(Guid walletId)
        {
            if (walletId == Guid.Empty)
                return null;

            return await _dbContext.Transactions.Where(w => w.WalletId == walletId).ToListAsync();
        }
    }
}
