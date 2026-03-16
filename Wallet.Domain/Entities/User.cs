using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public bool Active { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

        public User() { }

        public User(string name, string email, string username, string password)
        {
            if (string.IsNullOrEmpty(name))
                throw new DomainException("Require a valid name");

            if (string.IsNullOrEmpty(email)) 
                throw new DomainException("Require a vilid email");

            if (string.IsNullOrEmpty(username))
                throw new DomainException("Require a valid username");

            if (string.IsNullOrEmpty(username))
                throw new DomainException("Require a valid password");

            Name = name;
            Email = email;
            Username = username;
            Password = password;
            Active = true;
            DeactivatedAt = null;
        }

        public void Deactivate()
        {
            Active = false;
            DeactivatedAt = DateTime.UtcNow; 
        }
    }
}
