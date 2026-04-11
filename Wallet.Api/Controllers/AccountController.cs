using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Wallet.Api.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ApiControllerBase
    { 
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserRequest request)
        {
            return HandleResult(await _userService.CreateAsync(request));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var result = await _userService.LoginAsync(request);

            if (!result.IsSuccess)
                return ApiRequestResponse.ToActionResult(result);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result!.Value!.UserId.ToString()),
                new Claim(ClaimTypes.Email, result.Value.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HereIsMyJwtToken_A_Special_12345"));

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credential);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(ApiResult<object>.Ok(new { token = jwt }));
        }
    }
}
