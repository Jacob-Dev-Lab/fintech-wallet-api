using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Application.Dtos.Responses;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletApiDbContext _dbContext;
        private readonly ILogger<WalletRepository> _logger;

        public WalletRepository(WalletApiDbContext context, ILogger<WalletRepository> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------
        public void Add(WalletAccount wallet)
        {
            ArgumentNullException.ThrowIfNull(wallet);

            _logger.LogInformation(
               "Adding new wallet. WalletId={WalletId}, UserId={UserId}",
               wallet.WalletId, wallet.UserId);

            _dbContext.Wallets.Add(wallet);
        }

        // ---------------------------------------------------------
        // TRACKED QUERIES (for updates)
        // ---------------------------------------------------------

        public async Task<WalletAccount?> FindByWalletIdAsync(long userId, Guid walletId)
        {
            _logger.LogInformation(
               "Fetching tracked wallet. UserId={UserId}, WalletId={WalletId}",
               userId, walletId);

            var wallet = await _dbContext.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.WalletId == walletId);

            if (wallet is null)
                _logger.LogWarning(
                    "Tracked wallet not found. UserId={UserId}, WalletId={WalletId}",
                    userId, walletId);

            return wallet;
        }

        public async Task<WalletAccount?> FindByWalletIdAsync(Guid walletId)
        {
            _logger.LogInformation(
               "Fetching tracked wallet by ID only. WalletId={WalletId}",
               walletId);

            var wallet = await _dbContext.Wallets
                .FirstOrDefaultAsync(w => w.WalletId == walletId);

            if (wallet is null)
                _logger.LogWarning(
                    "Tracked wallet not found. WalletId={WalletId}",
                    walletId);

            return wallet;
        }

        // ---------------------------------------------------------
        // READ-ONLY QUERIES (no tracking)
        // ---------------------------------------------------------

        public async Task<WalletDto?> FindByWalletIdProjectionAsync(long userId, Guid walletId)
        {
            _logger.LogInformation(
               "Fetching wallet projection. UserId={UserId}, WalletId={WalletId}",
               userId, walletId);

            var wallet = await _dbContext.Wallets
                .AsNoTracking()
                .Where(w => w.UserId == userId && w.WalletId == walletId)
                .Select(WalletDto.Projection)
                .FirstOrDefaultAsync();

            if (wallet is null)
                _logger.LogWarning(
                    "Wallet not found. UserId={UserId}, WalletId={WalletId}",
                    userId, walletId);

            return wallet;
        }

        public async Task<IReadOnlyList<WalletDto>> FindByUserIdAsync(long userId)
        {
            _logger.LogInformation(
               "Fetching wallets by user ID. UserId={UserId}",
               userId);

            return await _dbContext.Wallets
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderBy(w => w.CreatedAt)
                .Select(WalletDto.Projection)
                .ToListAsync();
        }

        // ---------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------
        public void Update(WalletAccount wallet)
        {
            _logger.LogInformation(
               "Updating wallet. WalletId={WalletId}, UserId={UserId}",
               wallet.WalletId, wallet.UserId);

            _dbContext.Wallets.Update(wallet);
        }
    }
}
