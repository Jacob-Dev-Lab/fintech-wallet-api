using Wallet.Domain.Enums;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class Transaction
    {
        public long Id { get; private set; }
        public Guid WalletId { get; private set; }
        public Guid? DestinationWalletId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Transaction() { }

        public Transaction(TransactionType type, string description, Guid walletId, decimal amount, Guid? destinationWalletId = null)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be greater than zero.");

            if (!TransactionType.IsDefined(type))
                throw new DomainException("Invalid transaction type.");

            if (type == TransactionType.Transfer && destinationWalletId == null)
                throw new DomainException("Recipient wallet ID must be provided for transfer transactions.");

            WalletId = walletId;
            DestinationWalletId = destinationWalletId;
            Amount = amount;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            Description = description;
        }
    }
}
