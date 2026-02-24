namespace FileTable.Infrastructure.Abstractions
{
    public interface ITransactionDetector
    {
        bool IsTransactionActive();
    }
}
