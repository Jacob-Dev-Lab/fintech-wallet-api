using System.Net.Mail;
using System.Text.RegularExpressions;
using Wallet.Application.Interfaces;

namespace Wallet.Application.Utilities
{
    public class EmailValidator : IEmailValidator
    {
        private readonly Regex emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public bool IsValid(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            if (!emailRegex.IsMatch(email))
                return false;

            if (!MailAddress.TryCreate(email, out var emailAddress))
                return false;

            if (emailAddress.Address != email)
                return false;

            return true;
        }
    }
}
