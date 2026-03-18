using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletApiDbContext _dbContext;

        public WalletRepository(WalletApiDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddAsync(WalletAccount wallet)
        {
            ArgumentNullException.ThrowIfNull(wallet);

            await _dbContext.Wallets.AddAsync(wallet);
        }

        public async Task<WalletAccount?> GetByWalletIdAsync(Guid walletId)
        {
            return await _dbContext.Wallets
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public IQueryable<WalletAccount> GetByUserId(long userId)
        {
            return _dbContext.Wallets
                .AsNoTracking()
                .Where(w => w.UserId == userId);
        }
    }
}
