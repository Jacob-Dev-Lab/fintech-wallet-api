using System.ComponentModel.DataAnnotations;

namespace Wallet.Application.Dtos.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
    }
}
