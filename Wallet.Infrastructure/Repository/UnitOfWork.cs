using Wallet.Application.Interfaces;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WalletApiDbContext _dbContext;

        public UnitOfWork(WalletApiDbContext context)
        {
            _dbContext = context;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task ExecuteTransactionAsync(Func<Task> operation)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await operation();

                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
