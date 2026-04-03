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
                ErrorType.NotFound => new NotFoundObjectResult(new { errors = error.Message } ),
                ErrorType.BadRequest => new BadRequestObjectResult(new { errors = error.Message }),
                ErrorType.Conflict => new ConflictObjectResult(new { errors = error.Message }),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(new { errors = error.Message }),  
                _ => new StatusCodeResult(500)
            };
        }
    }
}
