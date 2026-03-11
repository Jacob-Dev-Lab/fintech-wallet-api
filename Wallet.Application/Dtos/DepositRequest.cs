using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos
{
    public class DepositRequest()
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
