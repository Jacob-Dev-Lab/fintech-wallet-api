using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos
{
    public class WalletCreationRequest
    {
        public long UserId { get; set; }
        public int Currency { get; set; }
    }
}
