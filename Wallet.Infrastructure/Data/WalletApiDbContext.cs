using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Data
{
    public class WalletApiDbContext : DbContext
    {
        public DbSet<User> Users { get; private set; }
        public DbSet<WalletAccount> Wallets { get; private set; }
        public DbSet<Transaction> Transactions { get; private set; }

        public WalletApiDbContext(DbContextOptions<WalletApiDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<WalletAccount>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Transaction>()
                .Property(t => t.Balance)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
