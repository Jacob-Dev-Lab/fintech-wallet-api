using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        Task<User?> FindByIdAsync(long Id);
        IQueryable<User> FindAll();
        Task<User?> FindByEmailAsync(string Email);
        void Update(User user);
    }
}
