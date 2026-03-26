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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _hasher;
        private readonly IEmailValidator _emailValidator;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher, IEmailValidator emailValidator)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hasher = passwordHasher;
            _emailValidator = emailValidator;
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) 
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<UserDto>
                    .Failure(new Error(ErrorType.BadRequest, "All fields are required"));
            }

            if (!_emailValidator.IsValid(request.Email))
                return Result<UserDto>
                       .Failure(new Error(ErrorType.BadRequest, "Invalid email address."));

            try
            {
                if (await _userRepository.FindByEmailAsync(request.Email) != null)
                    return Result<UserDto>
                           .Failure(new Error(ErrorType.BadRequest, "Email address already exist."));

                var passwordHash = _hasher.Hash(request.Password);

                var user = new User
                (
                    request.Name,
                    request.Email,
                    passwordHash
                );

                _userRepository.Add(user);
                await _unitOfWork.SaveChangesAsync();

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
        }

        public async Task<Result<UserDto>> GetByIdAsync(long Id)
        {
            var user = await _userRepository.FindByIdAsync(Id);

            if (user is null)
                return Result<UserDto>.Failure(new Error(ErrorType.NotFound, "Wallet not found."));

            return Result<UserDto>.Success(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

        public async Task<Result<IReadOnlyList<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository
                .FindAll()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();

            return Result<IReadOnlyList<UserDto>>.Success(users);
        }

        public async Task<Result<UserLoginDto>> LoginAsync(UserLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Result<UserLoginDto>
                    .Failure(new Error(ErrorType.BadRequest, "Usrname/Password required"));

            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.FindByEmailAsync(email);

            if (user is null)
                return Result<UserLoginDto>
                    .Failure(new Error(ErrorType.Unauthorized, "Incorrect Username/Password."));

            if (!_hasher.Verify(user.PasswordHash, request.Password))
                return Result<UserLoginDto>
                    .Failure(new Error(ErrorType.Unauthorized, "Incorrect Username/Password."));

            return Result<UserLoginDto>
                    .Success(new UserLoginDto 
                    { 
                        UserId = user.Id, 
                        Email = user.Email 
                    });
        }
    }
}
