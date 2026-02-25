using Dapper;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeePayrollReadOnly : IReadEmployeePayroll
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeePayrollReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EmployeePayrollDataModel>> ReadLastMonthsPayrollAsync(long employeeId, int months)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT TOP (@months) *
                FROM [documentdb].[dbo].[EmployeePayroll]
                WHERE EmployeeID = @employeeId
                ORDER BY PayPeriodYear DESC, PayPeriodMonth DESC";

            var entities = await connection.QueryAsync<EmployeePayrollEntity>(sql, new { employeeId, months });
            return entities.Select(MapToModel).ToList();
        }

        private static EmployeePayrollDataModel MapToModel(EmployeePayrollEntity e)
        {
            return new EmployeePayrollDataModel
            {
                BaseSalary = e.BaseSalary,
                Bonus = e.Bonus,
                CreatedDate = e.CreatedDate,
                Deductions = e.Deductions,
                EmployeeID = e.EmployeeID,
                ModifiedDate = e.ModifiedDate,
                NetSalary = e.NetSalary,
                Notes = e.Notes,
                PaymentDate = e.PaymentDate,
                PaymentMethod = e.PaymentMethod,
                PayPeriodMonth = e.PayPeriodMonth,
                PayPeriodYear = e.PayPeriodYear,
                PayrollID = e.PayrollID
            };
        }
    }
}
