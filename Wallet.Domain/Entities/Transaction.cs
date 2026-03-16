using Wallet.Domain.Enums;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class Transaction
    {
        public long Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Guid WalletId { get; private set; }
        public long UserId { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }
        public Guid? ReferenceWalletId { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Transaction() { }

        public Transaction(long userId, Guid walletId, TransactionType type, decimal amount,
            decimal balance, string? description = "-", Guid? referenceWalletId = null)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be greater than zero.");

            if (!Enum.IsDefined<TransactionType>(type))
                throw new DomainException("Invalid transaction type.");

            if ((type == TransactionType.TransferIn || type == TransactionType.TransferOut)
                && referenceWalletId == null)
                throw new DomainException("Transfer requires a reference wallet.");

            TransactionId = Guid.NewGuid();
            UserId = userId;
            WalletId = walletId;
            Type = type;
            Amount = amount;
            Balance = balance;
            ReferenceWalletId = referenceWalletId;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
