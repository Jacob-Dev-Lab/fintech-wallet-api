using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly StatusResponse _response;

        public TransactionsController(ITransactionService transactionService, StatusResponse response)
        {
            _transactionService = transactionService;
            _response = response;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId(long userId)
        {
            var result = await _transactionService.GetByUserIdAsync(userId);

            if (!result.IsSuccess)
                return _response.Action(result.Error!); ;

            return Ok(result.Value);
        }

        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetByWalletId(Guid walletId)
        {
            var result = await _transactionService.GetByWalletIdAsync(walletId);

            if (!result.IsSuccess)
                return _response.Action(result.Error!);

            return Ok(result.Value);
        }
    }
}
