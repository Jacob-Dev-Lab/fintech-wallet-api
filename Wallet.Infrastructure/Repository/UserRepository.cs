using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly WalletApiDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(WalletApiDbContext context, ILogger<UserRepository> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        public void Add(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            _logger.LogInformation("Adding new user with email: {Email}", user.Email);

            _dbContext.Users.Add(user);
        }

        // ---------------------------------------------------------
        // TRACKED QUERIES (for updates)
        // ---------------------------------------------------------
        public async Task<User?> FindByIdAsync(long Id)
        {
            _logger.LogInformation("Finding user by ID: {Id}", Id);

            var user = await _dbContext.Users.FindAsync(Id);

            if (user is null)
                _logger.LogWarning("Tracked user not found. UserId={UserId}", Id);

            return user;
        }

        // ---------------------------------------------------------
        // READ-ONLY QUERIES (no tracking)
        // ---------------------------------------------------------
        public IQueryable<User> FindAll()
        {
            _logger.LogInformation("Finding all users");

            return _dbContext.Users
                .AsNoTracking();
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            _logger.LogInformation("Finding user by email: {Email}", email);

            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
                _logger.LogWarning("No user found for this email. Email={Email}", email);

            return user;
        }

        // ---------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------
        public void Update(User user)
        {
            _logger.LogInformation("Updating user with ID: {Id}", user.Id);

            _dbContext.Users.Update(user);
        }
    }
}
