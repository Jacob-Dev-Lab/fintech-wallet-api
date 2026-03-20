using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos.Requests
{
    public class DepositRequest()
    {
        [Required]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }
    }
}
