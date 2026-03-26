using System.Net.Mail;
using System.Text.RegularExpressions;
using Wallet.Application.Interfaces;

namespace Wallet.Application.Utilities
{
    public partial class EmailValidator : IEmailValidator
    {
        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
        private static partial Regex EmailRegex();

        public bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex().IsMatch(email);
        }
    }
}
