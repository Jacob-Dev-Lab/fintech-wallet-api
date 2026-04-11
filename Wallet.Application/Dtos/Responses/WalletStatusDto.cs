using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Responses
{
    public class WalletStatusDto
    {
        public Guid WalletId { get; init; }
        public bool Active { get; init; }
    }
}
