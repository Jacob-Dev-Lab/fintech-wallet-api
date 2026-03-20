using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos.Requests
{
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; init; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(100)]
        public string? Password { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; init; }
    }
}
