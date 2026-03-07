using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Dtos;
using Wallet.Application.Interfaces;

namespace Wallet.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var result = await _service.CreateWallet();

            if (!result.IsSuccess || result.Value is null)
                return BadRequest(result.Message);
            
            var wallet = result.Value;

            return CreatedAtAction(
                nameof(Create),
                new WalletResponse 
                {
                    WalletId = wallet.WalletId, 
                    Balance = wallet.Balance
                }
            );
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(Guid id, [FromBody] DepositRequest request)
        {
            var result = await _service.Deposit(id, request);

            if (!result.IsSuccess || result.Value is null)
                return NotFound(result.Message);

            var wallet = result.Value;

            return Ok(new WalletResponse
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance
            });
        }
    }
}
