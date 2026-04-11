using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Common;
using Wallet.Application.Common;

namespace Wallet.Api.Controllers
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected long UserId =>
            long.TryParse(User.FindFirstValue(ClaimTypes.Name), out long userId) 
                ? userId : 0;
        protected IActionResult HandleResult<T>(Result<T> result) =>
            ApiRequestResponse.ToActionResult(result);

        protected IActionResult HandleCreatedResult<T>(Result<T> result, string location) =>
            ApiRequestResponse.ToCreateResult(location, result);
    }
}
