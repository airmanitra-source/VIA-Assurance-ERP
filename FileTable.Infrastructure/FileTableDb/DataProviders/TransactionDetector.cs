using System.Transactions;
using Company.Sinister.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class TransactionDetector : ITransactionDetector
    {
        public bool IsTransactionActive()
        {
            return Transaction.Current != null;
        }
    }
}
