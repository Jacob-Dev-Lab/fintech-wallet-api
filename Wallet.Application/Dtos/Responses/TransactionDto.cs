using System.Linq.Expressions;
using Wallet.Domain.Entities;

namespace Wallet.Application.Dtos.Responses
{
    public class TransactionDto
    {
        public DateTime DateCreated { get; set; }
        public Guid Id { get; set; }
        public string? Transaction { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public decimal Balance { get; set; }

        public static Expression<Func<Transaction, TransactionDto>> Projection =>
            transaction => new TransactionDto {
                DateCreated = transaction.CreatedAt,
                Id = transaction.TransactionId,
                Transaction = transaction.Type.ToString(),
                Amount = transaction.Amount,
                Description = transaction.Description,
                Balance = transaction.Balance
            };
    }
}
