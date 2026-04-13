using System.ComponentModel.DataAnnotations;
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

        [Timestamp]
        public byte[] RowVersion { get; private set; }

        //private WalletAccount() { } //EF Core Constructor

        public WalletAccount(long userId, Currency currency)
        {
            if (!Enum.IsDefined(typeof(Currency), currency))
                throw new DomainException("Require a valid currency type");

            UserId = userId;
            WalletId = Guid.NewGuid();
            Currency = currency;
            Balance = 0m;
            IsActive = true;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Freeze()
        {
            if (!IsActive)
                throw new DomainException("Wallet already frozen");

            IsActive = false;
        }

        public void UnFreeze()
        {
            if (IsActive)
                throw new DomainException("Wallet already Active");

            IsActive = true;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new DomainException("Wallet already deleted");

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

        private void CheckIfAmountIsGreaterThanZero(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be positive.");
        }

        private void CheckIfWalletIsActive()
        {
            if (!IsActive)
                throw new DomainException("Wallet is frozen.");
        }

        private void CheckIfWalletIsDeleted()
        {
            if (IsDeleted)
                throw new DomainException("Wallet does not exist");
        }

        public void Deposit(decimal amount)
        {
            CheckIfWalletIsDeleted();
            CheckIfWalletIsActive();
            CheckIfAmountIsGreaterThanZero(amount);

            ChangeBalance(amount);
        }
        public void Withdraw(decimal amount)
        {
            CheckIfWalletIsDeleted();
            CheckIfWalletIsActive();
            CheckIfAmountIsGreaterThanZero(amount);

            if (Balance < amount)
            {
                throw new DomainException("Insufficient balance.");
            }

            ChangeBalance(-amount);
        }

        public void TransferTo (WalletAccount target, decimal amount)
        {
            if (target == null)
                throw new DomainException("Invalid target wallet.");

            if (Currency != target.Currency)
                throw new DomainException("Currency mismatch.");

            if (WalletId == target.WalletId)
                throw new DomainException("Cannot transfer to the same wallet.");

            CheckIfWalletIsDeleted();
            target.CheckIfWalletIsDeleted();

            CheckIfWalletIsActive();
            target.CheckIfWalletIsActive();

            CheckIfAmountIsGreaterThanZero(amount);

            ChangeBalance(-amount);
            target.ChangeBalance(amount);
        }
    }
}