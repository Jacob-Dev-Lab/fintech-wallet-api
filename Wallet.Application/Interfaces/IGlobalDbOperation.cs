namespace Wallet.Application.Interfaces
{
    public interface IGlobalDbOperation
    {
        Task SaveChangesAsync();
        Task ExecuteTransactionAsync(Func<Task> operation);
    }
}
