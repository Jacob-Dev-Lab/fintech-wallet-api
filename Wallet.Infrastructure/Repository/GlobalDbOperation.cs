using Wallet.Application.Interfaces;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class GlobalDbOperation : IGlobalDbOperation
    {
        private readonly WalletApiDbContext _dbContext;

        public GlobalDbOperation(WalletApiDbContext context)
        {
            _dbContext = context;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
