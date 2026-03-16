using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos
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
        [MinLength(5)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$",
            ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public string? Username { get; init; }

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
