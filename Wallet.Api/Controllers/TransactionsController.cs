using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ApiControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId()
        {
            return HandleResult(await _transactionService.GetByUserIdAsync(UserId));
        }

        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetByWalletId(Guid walletId)
        {
            return HandleResult(await _transactionService.GetByWalletIdAsync(UserId, walletId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByTransactionId(Guid id)
        {
            return HandleResult(await _transactionService.GetByTransactionIdAsync(UserId, id));
        }
    }
}

