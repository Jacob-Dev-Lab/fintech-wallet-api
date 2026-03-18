using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos.Requests
{
    public class CreateWalletRequest
    {
        public long UserId { get; set; }

        [Required]
        public int Currency { get; set; }
    }
}
