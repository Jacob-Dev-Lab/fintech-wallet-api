using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Responses
{
    public class TransactionDto
    {
        public DateTime DateCreated { get; set; }
        public string? Transaction { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public decimal Balance { get; set; }

    }
}
