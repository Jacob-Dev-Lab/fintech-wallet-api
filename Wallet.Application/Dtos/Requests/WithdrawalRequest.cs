using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Requests
{
    public class WithdrawalRequest
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }
    }
}
