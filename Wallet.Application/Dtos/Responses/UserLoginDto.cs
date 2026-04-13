using System.Linq.Expressions;
using Wallet.Domain.Entities;

namespace Wallet.Application.Dtos.Responses
{
    public class UserLoginDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;

        public static Expression<Func<User, UserLoginDto>> Projection =>
            user => new UserLoginDto {
                Id = user.Id,
                Email = user.Email,
                Hash = user.PasswordHash
            };
    }
}
