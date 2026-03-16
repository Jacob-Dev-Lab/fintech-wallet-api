using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Common;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controller
{
    [Controller]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId(long userId)
        {
            var result = await _transactionService.GetByUserIdAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Value);
        }

        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetByWalletId(Guid walletId)
        {
            var result = await _transactionService.GetByWalletIdAsync(walletId);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Value);
        }
    }
}
