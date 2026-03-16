using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controller
{
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
        public async Task<IActionResult> GetAll(long userId)
        {
            var result = await _service.GetByUserIdAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid walletId)
        {
            var result = await _service.GetByWalletIdAsync(walletId);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WalletCreationRequest request)
        {
            var result = await _service.CreateAsync(request.UserId, request.Currency);

            if (!result.IsSuccess || result.Value is null)
                return BadRequest(result.Message);

            var wallet = result.Value;

            return Created($"api/wallets/{wallet.WalletId}", wallet);
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(Guid walletId, [FromBody] DepositRequest request)
        {
            var result = await _service.DepositAsync(walletId, request);

            if (!result.IsSuccess || result.Value is null)
                return NotFound(result.Message);

            return Ok(result.Value);
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid id, [FromBody] WithdrawalRequest request)
        {
            var result = await _service.WithdrawAsync(id, request);

            if (!result.IsSuccess || result.Value is null)
                return NotFound(result.Message);

            return Ok(result.Value);
        }

        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> Transfer(Guid id, [FromBody] TransferRequest request)
        {
            var result = await _service.TransferAsync(id, request);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Value);
        }
    }
}
