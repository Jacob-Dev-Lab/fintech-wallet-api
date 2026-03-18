using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
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
        public async Task<IActionResult> GetUserAsync(long Id)
        {
            var result = await _userService.GetByIdAsync(Id);

            if (!result.IsSuccess)
                return _response.Action(result.Error!);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.AddAsync(request);

            if (!result.IsSuccess)
                return _response.Action(result.Error!);

            return Created(nameof(result), result.Value);
        }
    }
}
