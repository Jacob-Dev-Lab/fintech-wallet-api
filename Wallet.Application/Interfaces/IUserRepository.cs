using Wallet.Domain.Entities;

namespace Wallet.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        IQueryable<User> GetAll();
    }
}
