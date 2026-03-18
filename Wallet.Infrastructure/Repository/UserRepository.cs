using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly WalletApiDbContext _dbContext;

        public UserRepository(WalletApiDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            await _dbContext.Users.AddAsync(user);
        }

        //public Task UpdateAsync(User user)
        //{
        //    ArgumentNullException.ThrowIfNull(user);

        //    _dbContext.Users.Update(user);
        //    return Task.CompletedTask;
        //}

        public async Task<User?> GetByIdAsync(int Id)
        {
            return await _dbContext.Users.FindAsync(Id);
        }

        public IQueryable<User> GetAll()
        {
            return _dbContext.Users.AsNoTracking();
        }
    }
}
