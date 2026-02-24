using System.Transactions;
using FileTable.Infrastructure.Abstractions;

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
