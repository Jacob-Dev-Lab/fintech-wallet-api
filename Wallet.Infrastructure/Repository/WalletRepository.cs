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

        public async Task Add(WalletAccount wallet)
        {
            if (wallet is null)
                throw new ArgumentNullException(nameof(wallet), "Invalid entry, try again.");

            await _dbContext.Wallets.AddAsync(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<WalletAccount?> GetById(Guid walletId)
        {
            if (walletId.Equals(Guid.Empty))
                throw new ArgumentException("Invalid entry, try again.", nameof(walletId));

            return await _dbContext.Wallets.FirstOrDefaultAsync(w => w.WalletId.Equals(walletId));
        }

        public async Task Update(WalletAccount wallet)
        {
            if (wallet is null)
                throw new ArgumentNullException(nameof(wallet), "Invalid entry, try again.");

            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
