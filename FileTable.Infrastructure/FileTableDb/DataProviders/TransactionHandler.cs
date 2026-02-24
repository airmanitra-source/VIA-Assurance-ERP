using System.Transactions;
using FileTable.Infrastructure.Abstractions;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class TransactionHandler : ITransactionHandler
    {
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await action();
            scope.Complete();
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await action();
            scope.Complete();
            return result;
        }
    }
}
