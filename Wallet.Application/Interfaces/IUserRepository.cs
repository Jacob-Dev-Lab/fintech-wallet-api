using Wallet.Application.Dtos.Responses;
using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        Task<User?> FindByIdForUpdateAsync(long Id);
        Task<UserDto?> FindByIdAsync(long Id);
        Task<IReadOnlyList<UserDto>> FindAllAsync();
        Task<UserLoginDto?> FindByEmailAsync(string Email);
        void Update(User user);
    }
}
