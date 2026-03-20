using Microsoft.AspNetCore.Identity;
using Wallet.Application.Interfaces;

namespace Wallet.Application.Utilities
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hash = new();

        public string Hash(string password)
        {
            return _hash.HashPassword(null!, password);
        }

        public bool Verify(string hash, string password)
        {
            var result = _hash.VerifyHashedPassword(null!, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
