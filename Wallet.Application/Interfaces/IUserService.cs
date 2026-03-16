using Wallet.Application.Common;
using Wallet.Application.Dtos;

namespace Wallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponse>> CreateAsync(CreateUserRequest requst);
        Task<Result<IReadOnlyList<UserResponse>>> GetUsersAsync();
    }
}
