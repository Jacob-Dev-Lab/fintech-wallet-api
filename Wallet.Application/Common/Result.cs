namespace Wallet.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Message { get; }

        public Result(bool isSuccess, string? message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success() => new (true, null);
        public static Result Failure(string message) => new (false, message);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }
        private Result(T value) : base(true, null) => Value = value;
        private Result(string message) : base(false, message) { }
        public static Result<T> Success(T value) => new (value);
        public static new Result<T> Failure(string message) => new (message);
    }
}
