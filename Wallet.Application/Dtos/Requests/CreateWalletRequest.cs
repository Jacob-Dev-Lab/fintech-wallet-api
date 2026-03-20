using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos.Requests
{
    public class CreateWalletRequest
    {
        [Required]
        public int Currency { get; set; }
    }
}
