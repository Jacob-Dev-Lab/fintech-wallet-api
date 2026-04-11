using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Wallet.Api.Validators
{
    public class GlobalValidationFactory : IFluentValidationAutoValidationResultFactory
    {
        public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, 
            ValidationProblemDetails validationProblemDetails, 
            IDictionary<IValidationContext, ValidationResult> validationResults)
        {
            var errors = validationResults.SelectMany(vr => vr.Value.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new ApiResult<object>
            {
                Success = false,
                Data = null,
                Error = new ApiError("ValidationError", errors)
            };

            return Task.FromResult<IActionResult?>(new BadRequestObjectResult(response));
        }
    }
}
