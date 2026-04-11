using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Common;
using Wallet.Application.Common.Enum;

namespace Wallet.Api.Common
{
    public static class ApiRequestResponse
    {
        public static IActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(ApiResult<T>.Ok(result.Value!));
            }

            return result.Error!.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(
                    ApiResult<T>.Fail("NotFound", result.Error.Messages)),
                ErrorType.BadRequest => new BadRequestObjectResult(
                    ApiResult<T>.Fail("BadRequest", result.Error.Messages)),
                ErrorType.Conflict => new ConflictObjectResult(
                    ApiResult<T>.Fail("Conflict", result.Error.Messages)),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(
                    ApiResult<T>.Fail("Unauthorized", result.Error.Messages)),
                _ => new BadRequestObjectResult(
                    ApiResult<T>.Fail("UnknownError", result.Error.Messages))
            };
        }

        public static IActionResult ToCreateResult<T>(string location, Result<T> result)
        {
            if (!result.IsSuccess)
                return ToActionResult(result);

            return new CreatedResult(location, ApiResult<T>.Created(result.Value!));
        }
    }
}
