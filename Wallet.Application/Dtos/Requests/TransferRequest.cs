using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Requests
{
    public class TransferRequest
    {
        [Required]
        public Guid ReceivingWalletId {  get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
