using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    { 
        private readonly IUserService _userService;
        private readonly StatusResponse _response;

        public AccountController(IUserService userService, StatusResponse response)
        {
            _userService = userService;
            _response = response;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.CreateAsync(request);

            if (!result.IsSuccess)
                return _response.Action(result.Error!);

            return Created(nameof(result), result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var result = await _userService.LoginAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

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
