using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Common;

namespace Wallet.Api
{
    public class StatusResponse
    {
        public IActionResult Action(Error error)
        {
            return error.errorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(error.message),
                ErrorType.BadRequest => new BadRequestObjectResult(error.message),
                ErrorType.Conflict => new ConflictObjectResult(error.message),
                _ => new StatusCodeResult(500)
            };
        }
    }
}
