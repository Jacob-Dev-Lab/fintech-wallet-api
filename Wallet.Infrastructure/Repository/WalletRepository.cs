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

        public void Add(WalletAccount wallet)
        {
            ArgumentNullException.ThrowIfNull(wallet);

            _dbContext.Wallets.Add(wallet);
        }

        public async Task<WalletAccount?> FindByWalletIdAsync(long userId, Guid walletId)
        {
            return await _dbContext.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.WalletId == walletId);
        }

        public async Task<WalletAccount?> FindByWalletIdAsync(Guid walletId)
        {
            return await _dbContext.Wallets
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public IQueryable<WalletAccount> FindByUserId(long userId)
        {
            return _dbContext.Wallets
                .AsNoTracking()
                .Where(w => w.UserId == userId);
        }

        public void Update(WalletAccount wallet)
        {
            _dbContext.Wallets.Update(wallet);
        }
    }
}
