using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletsController : ApiControllerBase
    {
        private readonly IWalletService _service;

        public WalletsController(IWalletService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await _service.GetByUserIdAsync(UserId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return HandleResult(await _service.GetByWalletIdAsync(UserId, id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
        {
            var result = await _service.CreateAsync(UserId, request.Currency);
            return HandleCreatedResult(result, $"api/wallets/{result.Value!.WalletId}");
        }

        [HttpPost("{id}/freeze")]
        public async Task<IActionResult> Freeze(Guid id)
        {
            return HandleResult(await _service.FreezeWalletAsync(UserId, id));
        }

        [HttpPost("{id}/unfreeze")]
        public async Task<IActionResult> Unfreeze(Guid id)
        {
            return HandleResult(await _service.UnfreezeWalletAsync(UserId, id));
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositRequest request)
        {
            return HandleResult(await _service.DepositAsync(UserId, id, request));
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid id, [FromBody] WithdrawRequest request)
        {
            return HandleResult(await _service.WithdrawAsync(UserId, id, request));
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            return HandleResult(await _service.TransferAsync(UserId, request));
        }
    }
}
