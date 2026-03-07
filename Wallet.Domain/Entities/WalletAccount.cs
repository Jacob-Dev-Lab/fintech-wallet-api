using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class WalletAccount
    {
        public long Id { get; private set; }
        public Guid WalletId { get; private set; }
        public decimal Balance { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public WalletAccount() { } //EF Core Constructor

        public WalletAccount(Guid walletId, DateTime createdAt, decimal balance = 0m)
        {
            if (balance < 0)
                throw new DomainException("Amount must be greater than zero.");

            WalletId = walletId;
            Balance = balance;
            IsActive = true;
            IsDeleted = false;
            CreatedAt = createdAt;
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
                throw new DomainException("Wallet is not active.");
        }

        private void EnssureWalletIsNotDeleted()
        {
            if (IsDeleted)
                throw new DomainException("Wallet is deleted.");
        }
    }
}
