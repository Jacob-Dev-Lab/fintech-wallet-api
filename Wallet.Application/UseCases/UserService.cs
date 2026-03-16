using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Common;
using Wallet.Application.Dtos;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;

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

        public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request)
        {
            var user = new User
                (
                    request.Name,
                    request.Email,
                    request.Username,
                    request.Password
                );

            if (user is null)
                return Result<UserResponse>.Failure("Reaqure valid user");

            await _userRepository.UpdateAsync(user);
            await _dbOperation.SaveChangesAsync();

            return Result<UserResponse>
                .Success(new UserResponse 
                { 
                    Id = user.Id, 
                    Name = user.Name, 
                    Email = user.Email
                });
        }

        public async Task<Result<IReadOnlyList<UserResponse>>> GetUsersAsync()
        {
            var users = await _userRepository.GetAll()
                .Select(u => new UserResponse 
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();

            return Result<IReadOnlyList<UserResponse>>.Success(users);
        }
    }
}
