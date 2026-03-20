namespace Wallet.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        Task ExecuteTransactionAsync(Func<Task> operation);
    }
}
