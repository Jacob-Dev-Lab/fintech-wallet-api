using FluentValidation.Results;

namespace Wallet.Api.Common
{
    public static class Extensions
    {
        public static object ToErrorResponse(this ValidationResult result)
        {
            var errors = new List<string>();

            return new
            {
                errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray())
            };
        }
    }
}
