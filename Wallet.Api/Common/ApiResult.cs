using Wallet.Application.Common;

namespace Wallet.Api.Common
{
    public class ApiResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ApiError? Error { get; init; }

        public static ApiResult<T> Ok(T data)
        {
            return new()
            {
                Success = true,
                Data = data,
                Error = null
            };
        }

        public static ApiResult<T> Created(T data)
        {
            return new()
            {
                Success = true,
                Data = data,
                Error = null
            };
        }

        public static ApiResult<T> Fail(string type, IEnumerable<string> messages)
        {
            return new()
            {
                Success = false,
                Data = default,
                Error = new ApiError(type, messages)
            };
        }
    }

    public record ApiError(string Type, IEnumerable<string> Messages);
}
