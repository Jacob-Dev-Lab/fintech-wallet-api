using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly StatusResponse _response;

        public UsersController(IUserService userService, StatusResponse response)
        {
            _userService = userService;
            _response = response;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _userService.GetAllAsync();

            if (!result.IsSuccess)
                return _response.Action(result.Error!);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync()
        {
            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _userService.GetByIdAsync(userId);

            if (!result.IsSuccess)
                return _response.Action(result.Error!);

            return Ok(result.Value);
        }
    }
}
