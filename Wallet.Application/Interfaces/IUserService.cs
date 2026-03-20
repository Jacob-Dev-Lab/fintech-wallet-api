using Wallet.Application.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Responses;

namespace Wallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> CreateAsync(CreateUserRequest requst);
        Task<Result<UserDto>> GetByIdAsync(long Id);
        Task<Result<IReadOnlyList<UserDto>>> GetAllAsync();
        Task<Result<UserLoginDto>> LoginAsync(UserLoginRequest request);
    }
}
