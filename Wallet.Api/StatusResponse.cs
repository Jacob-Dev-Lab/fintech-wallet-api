using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Common;

namespace Wallet.Api
{
    public static class StatusResponse
    {
        public static IActionResult ToActionResult(Error error)
        {
            return error.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(error.Message),
                ErrorType.BadRequest => new BadRequestObjectResult(error.Message),
                ErrorType.Conflict => new ConflictObjectResult(error.Message),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(error.Message),
                _ => new StatusCodeResult(500)
            };
        }
    }
}
