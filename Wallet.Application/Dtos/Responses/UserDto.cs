using System.Linq.Expressions;
using Wallet.Domain.Entities;

namespace Wallet.Application.Dtos.Responses
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? DeactivatedAt { get; set; }

        public static Expression<Func<User, UserDto>> Projection =>
            user => new UserDto {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                DeactivatedAt = user.DeactivatedAt
            };
        
    }
}
