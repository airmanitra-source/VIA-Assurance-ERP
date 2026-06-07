using Dapper;
using EmployeePayroll.Module.Data.Models;
using EmployeePayroll.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeePayrollReadWrite : IEmployeePayrollReadWriteDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeePayrollReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreatePayrollAsync(EmployeePayrollDataModel payroll)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EmployeePayroll] 
                (EmployeeID, PayPeriodMonth, PayPeriodYear, BaseSalary, Bonus, Deductions, PaymentDate, PaymentMethod, Notes)
                VALUES 
                (@EmployeeID, @PayPeriodMonth, @PayPeriodYear, @BaseSalary, @Bonus, @Deductions, @PaymentDate, @PaymentMethod, @Notes);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, payroll);
        }

        public async Task UpdatePayrollAsync(EmployeePayrollDataModel payroll)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EmployeePayroll]
                SET BaseSalary = @BaseSalary, Bonus = @Bonus, Deductions = @Deductions,
                    PaymentDate = @PaymentDate, PaymentMethod = @PaymentMethod, Notes = @Notes,
                    ModifiedDate = GETDATE()
                WHERE PayrollID = @PayrollID";
            await connection.ExecuteAsync(sql, payroll);
        }

        public async Task DeletePayrollAsync(int payrollId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                DELETE FROM [documentdb].[dbo].[PaySlipLine] WHERE PayrollID = @PayrollID;
                DELETE FROM [documentdb].[dbo].[EmployeePayroll] WHERE PayrollID = @PayrollID;
            ";
            await connection.ExecuteAsync(sql, new { PayrollID = payrollId });
        }
    }
}

