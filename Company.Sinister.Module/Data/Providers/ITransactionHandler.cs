namespace Company.Sinister.Module.Data.Providers
{
    public interface ITransactionHandler
    {
        Task ExecuteInTransactionAsync(Func<Task> action);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
