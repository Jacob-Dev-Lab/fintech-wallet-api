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

        public void Add(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            _dbContext.Users.Add(user);
        }

        public async Task<User?> FindByIdAsync(long Id)
        {
            return await _dbContext.Users.FindAsync(Id);
        }

        public IQueryable<User> FindAll()
        {
            return _dbContext.Users.AsNoTracking();
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public void Update(User user)
        {
            _dbContext.Users.Update(user);
        }
    }
}
