using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Application.Common;
using Wallet.Application.Common.Enum;
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
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher, IEmailValidator emailValidator,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hasher = passwordHasher;
            _emailValidator = emailValidator;
            _logger = logger;
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request)
        {
            _logger.LogInformation("User registration attempt. Email={Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) 
                || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("User registration failed due to missing fields. Email={Email}", 
                    request.Email);

                return Result<UserDto>
                    .Failure(new Error(ErrorType.BadRequest, ["All fields are required"]));
            }

            if (!_emailValidator.IsValid(request.Email))
            {
                _logger.LogWarning("User registration failed due to invalid email. Email={Email}", 
                    request.Email);

                return Result<UserDto>
                       .Failure(new Error(ErrorType.BadRequest, ["Invalid email address."]));
            }

            try
            {
                if (await _userRepository.FindByEmailAsync(request.Email) != null)
                {
                    _logger.LogWarning("User registration failed due to existing email. Email={Email}", 
                        request.Email);

                    return Result<UserDto>
                           .Failure(new Error(ErrorType.BadRequest, ["Email address already exist."]));
                }

                var passwordHash = _hasher.Hash(request.Password);

                var user = new User
                (
                    request.Name,
                    request.Email,
                    passwordHash
                );

                _userRepository.Add(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User registration successful. UserId={UserId}, Email={Email}", 
                    user.Id, request.Email);

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
                _logger.LogError(ex, "Domain error during user registration. Email={Email}", 
                    request.Email);

                return Result<UserDto>.Failure(new Error(ErrorType.BadRequest, [ex.Message]));
            }
        }

        public async Task<Result<UserDto>> GetByIdAsync(long Id)
        {
            var user = await _userRepository.FindByIdAsync(Id);

            if (user is null)
                return Result<UserDto>.Failure(new Error(ErrorType.NotFound, ["Wallet not found."]));

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<IReadOnlyList<UserDto>>> GetAllAsync()
        {
            var users = await _userRepository
                .FindAllAsync();

            return Result<IReadOnlyList<UserDto>>.Success(users);
        }

        public async Task<Result<UserLoginDto>> LoginAsync(UserLoginRequest request)
        {
            _logger.LogInformation("User login attempt. Email={Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("User login failed due to missing fields. Email={Email}", 
                    request.Email);

                return Result<UserLoginDto>
                    .Failure(new Error(ErrorType.BadRequest, ["Usrname/Password required"]));
            }

            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogWarning("User login failed due to incorrect email. Email={Email}", 
                    request.Email);

                return Result<UserLoginDto>
                    .Failure(new Error(ErrorType.Unauthorized, ["Incorrect Username/Password."]));
            }

            if (!_hasher.Verify(user.Hash, request.Password))
            {
                _logger.LogWarning("User login failed due to incorrect password. Email={Email}", 
                    request.Email);

                return Result<UserLoginDto>
                    .Failure(new Error(ErrorType.Unauthorized, ["Incorrect Username/Password."]));
            }

            _logger.LogInformation("User login successful. UserId={UserId}, Email={Email}", 
                user.Id, request.Email);

            return Result<UserLoginDto>
                    .Success(new UserLoginDto 
                    { 
                        Id = user.Id, 
                        Email = user.Email 
                    });
        }
    }
}
