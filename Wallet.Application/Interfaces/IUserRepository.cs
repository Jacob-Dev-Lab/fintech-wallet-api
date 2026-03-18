using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(int Id);
        IQueryable<User> GetAll();
    }
}
