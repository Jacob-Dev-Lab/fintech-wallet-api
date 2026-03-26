using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos.Requests
{
    public class CreateWalletRequest
    {
        public int Currency { get; set; }
    }
}
