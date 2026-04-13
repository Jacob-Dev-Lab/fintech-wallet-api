using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class User
    {
        public long Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public bool Active { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

        //public User() { }

        public User(string name, string email, string passwordHash)
        {
            if (string.IsNullOrEmpty(name))
                throw new DomainException("Require a valid name");

            if (string.IsNullOrEmpty(email)) 
                throw new DomainException("Require a vilid email");

            if (string.IsNullOrEmpty(passwordHash))
                throw new DomainException("Require a valid password");

            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Active = true;
            DeactivatedAt = null;
        }

        public void Deactivate()
        {
            if (!Active)
                throw new DomainException("User already deactivated");

            Active = false;
            DeactivatedAt = DateTime.UtcNow; 
        }
    }
}
