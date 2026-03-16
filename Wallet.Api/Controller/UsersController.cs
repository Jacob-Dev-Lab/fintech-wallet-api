using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;

namespace Wallet.Api.Controller
{
    [Controller]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _userService.GetUsersAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Created("", result.Value);
        }
    }
}
