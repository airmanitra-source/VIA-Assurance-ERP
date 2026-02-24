namespace FileTable.Infrastructure.Abstractions
{
    public interface ITransactionHandler
    {
        Task ExecuteInTransactionAsync(Func<Task> action);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
