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
            await _dbContext.SaveChangesAsync();
        }

        public async Task<WalletAccount?> GetByIdAsync(Guid walletId)
        {
            if (walletId == Guid.Empty)
                return null;

            return await _dbContext.Wallets.FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public async Task<IReadOnlyList<WalletAccount>> GetAllAsync()
        {
            return await _dbContext.Wallets.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(WalletAccount wallet)
        {
            ArgumentNullException.ThrowIfNull(wallet);

            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();
        }

        //public async Task DeleteAsync(Guid walletId)
        //{
        //    if (walletId == Guid.Empty)
        //        throw new ArgumentNullException(nameof(walletId));

        //    var wallet = await _dbContext.Wallets.FindAsync(walletId);

        //    ArgumentNullException.ThrowIfNull(wallet);

        //    _dbContext.Wallets.Remove(wallet);
        //    await _dbContext.SaveChangesAsync();
        //}
    }
}
