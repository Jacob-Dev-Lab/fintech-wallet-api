using System.Data;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Responses;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;

namespace Wallet.Application.UseCases
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalDbOperation _dbOperation;

        public UserService(IUserRepository userRepository, IGlobalDbOperation operation)
        {
            _userRepository = userRepository;
            _dbOperation = operation;
        }

        public async Task<Result<UserDto>> AddAsync(CreateUserRequest request)
        {
            try
            {
                var user = new User
                (
                    request.Name,
                    request.Email,
                    request.Username,
                    request.Password
                );

                await _userRepository.AddAsync(user);
                await _dbOperation.SaveChangesAsync();

                return Result<UserDto>
                    .Success(new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email
                    });
            }
            catch (DomainException ex)
            {
                return Result<UserDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<UserDto>> GetByIdAsync(long Id)
        {
            var id = Int32.Parse(Id.ToString()); // to be corrected
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user is null)
                    return Result<UserDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found."));

                return Result<UserDto>.Success(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });
            }
            catch (DomainException ex)
            {
                return Result<UserDto>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<UserDto>>> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAll()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();

                return Result<IReadOnlyList<UserDto>>.Success(users);
            }
            catch (DomainException ex)
            {
                return Result<IReadOnlyList<UserDto>>.Failure(new Error(ErrorType.BadRequest, ex.Message));
            }
            catch
            {
                throw;
            }
        }
    }
}
