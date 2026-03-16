using Wallet.Domain.Enums;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class WalletAccount
    {
        public long Id { get; private set; }
        public long UserId { get; private set; }
        public Guid WalletId { get; private set; }
        public Currency Currency { get; private set; }
        public decimal Balance { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public WalletAccount() { } //EF Core Constructor

        public WalletAccount(long userId, Currency currency)
        {
            if (!Currency.IsDefined(currency))
                throw new DomainException("Require a valid currency type");

            UserId = userId;
            WalletId = Guid.NewGuid();
            Currency = currency;
            Balance = 0m;
            IsActive = true;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deposit(decimal amount)
        {
            EnssureWalletIsNotDeleted();
            EnsureWalletIsActive();
            EnsureAmountIsPositive(amount);

            ChangeBalance(amount);
        }
        public void Withdraw(decimal amount)
        {
            EnssureWalletIsNotDeleted();
            EnsureWalletIsActive();
            EnsureAmountIsPositive(amount);

            if (Balance < amount)
            {
                throw new DomainException("Insufficient balance.");
            }
            
            ChangeBalance(-amount);
        }

        public void Freez()
        {
            IsActive = false;
        }
        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;
        }

        private void ChangeBalance(decimal value)
        {
            var newBalance = Balance + value;

            if (newBalance < 0)
                throw new DomainException("Balance cannot be negative.");

            Balance = newBalance;
        }

        private void EnsureAmountIsPositive(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be positive.");
        }

        private void EnsureWalletIsActive()
        {
            if (!IsActive)
                throw new DomainException("Wallet is frozen.");
        }

        private void EnssureWalletIsNotDeleted()
        {
            if (IsDeleted)
                throw new DomainException("Wallet does not exist");
        }
    }
}
