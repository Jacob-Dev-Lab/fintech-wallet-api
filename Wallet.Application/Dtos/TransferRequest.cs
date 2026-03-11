using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos
{
    public class TransferRequest
    {
        public Guid ReceivingWalletId {  get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
    }
}
