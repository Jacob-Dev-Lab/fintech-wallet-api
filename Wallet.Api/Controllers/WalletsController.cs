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
        public async Task<IActionResult> GetById(Guid walletId)
        {
            return HandleResult(await _service.GetByWalletIdAsync(UserId, walletId));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
        {
            var result = await _service.CreateAsync(UserId, request.Currency);
            return HandleCreatedResult(result, $"api/wallets/{result.Value!.WalletId}");
        }

        [HttpPost("{walletId}/freeze")]
        public async Task<IActionResult> Freeze(Guid walletId)
        {
            return HandleResult(await _service.FreezWalletAsync(UserId, walletId));
        }

        [HttpPost("{walletId}/unfreeze")]
        public async Task<IActionResult> Unfreeze(Guid walletId)
        {
            return HandleResult(await _service.UnfreezWalletAsync(UserId, walletId));
        }

        [HttpPost("{walletId}/deposit")]
        public async Task<IActionResult> Deposit(Guid walletId, [FromBody] DepositRequest request)
        {
            return HandleResult(await _service.DepositAsync(UserId, walletId, request));
        }

        [HttpPost("{walletId}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid walletId, [FromBody] WithdrawalRequest request)
        {
            return HandleResult(await _service.WithdrawAsync(UserId, walletId, request));
        }

        [HttpPost("{walletId}/transfer")]
        public async Task<IActionResult> Transfer(Guid walletId, [FromBody] TransferRequest request)
        {
            return HandleResult(await _service.TransferAsync(UserId, walletId, request));
        }
    }
}
