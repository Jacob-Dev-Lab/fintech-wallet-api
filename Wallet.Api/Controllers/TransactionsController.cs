using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId()
        {
            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _transactionService.GetByUserIdAsync(userId);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!); ;

            return Ok(result.Value);
        }

        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetByWalletId(Guid walletId)
        {
            var userClaim = User.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userClaim, out long userId))
                return Unauthorized(userId);

            var result = await _transactionService.GetByWalletIdAsync(userId, walletId);

            if (!result.IsSuccess)
                return StatusResponse.ToActionResult(result.Error!);

            return Ok(result.Value);
        }
    }
}
