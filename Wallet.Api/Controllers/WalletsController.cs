using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletsController(IWalletService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _service.GetByUserIdAsync(userId);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!);

            return Ok(result.Value);
        }

        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetById(Guid walletId)
        {
            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _service.GetByWalletIdAsync(userId, walletId);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
        {
            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _service.CreateAsync(userId, request.Currency);

            if (!result.IsSuccess || result.Value is null)
                return StatusResponse.ToActionResult(result.Error!);

            var wallet = result.Value;

            return Created($"api/wallets/{wallet.WalletId}", wallet);
        }

        [HttpPost("{walletId}/deposit")]
        public async Task<IActionResult> Deposit(Guid walletId, [FromBody] DepositRequest request,
            IValidator<DepositRequest> validator)
        {
            var validatorResult = validator.Validate(request);

            if (!validatorResult.IsValid)
                return BadRequest(validatorResult.ToErrorResponse());

            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _service.DepositAsync(userId, walletId, request);

            if (!result.IsSuccess || result.Value is null)
                return StatusResponse.ToActionResult(result.Error!);

            return Ok(result.Value);
        }

        [HttpPost("{walletId}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid walletId, [FromBody] WithdrawalRequest request,
            IValidator<WithdrawalRequest> validator)
        {
            var validatorResult = validator.Validate(request);

            if (!validatorResult.IsValid)
                return BadRequest(validatorResult.ToErrorResponse());

            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _service.WithdrawAsync(userId, walletId, request);

            if (!result.IsSuccess || result.Value is null)
                return StatusResponse.ToActionResult(result.Error!);

            return Ok(result.Value);
        }

        [HttpPost("{walletId}/transfer")]
        public async Task<IActionResult> Transfer(Guid walletId, [FromBody] TransferRequest request,
            [FromServices] IValidator<TransferRequest> validator)
        {
            var validatorResult = validator.Validate(request);

            if (!validatorResult.IsValid)
                return BadRequest(validatorResult.ToErrorResponse());

            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _service.TransferAsync(userId, walletId, request);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!);

            return Ok(result.Value);
        }
    }
}
