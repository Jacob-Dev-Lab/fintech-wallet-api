using Wallet.Application.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Responses;

namespace Wallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserDto>> AddAsync(CreateUserRequest requst);
        Task<Result<UserDto>> GetByIdAsync(long Id);
        Task<Result<IReadOnlyList<UserDto>>> GetAllAsync();
    }
}
