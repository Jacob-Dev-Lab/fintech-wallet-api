using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Wallet.Api.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    { 
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserRequest request,
            IValidator<CreateUserRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToErrorResponse());

            var result = await _userService.CreateAsync(request);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!);

            return CreatedAtAction(nameof(RegisterAsync), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request,
            IValidator<UserLoginRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToErrorResponse());

            var result = await _userService.LoginAsync(request);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.Value!.UserId.ToString()),
                new Claim(ClaimTypes.Name, result.Value.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HereIsMyJwtToken_A_Special_12345"));

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credential);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwt });
        }
    }
}
