using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FileTable.Infrastructure
{
    public class FileTableDbContext
    {
        private readonly string _connectionString;

        public FileTableDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("FileTableConnection")
                ?? throw new InvalidOperationException("Connection string 'FileTableConnection' not found.");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
