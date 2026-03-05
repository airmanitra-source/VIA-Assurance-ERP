using CompanyPayroll.Module.Data.Models;
using CompanyPayroll.Module.Data.Providers;
using Dapper;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanyPayrollSettingsReadOnly : ICompanyPayrollSettingsReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public CompanyPayrollSettingsReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CompanyPayrollSettingsDataModel?> ReadByEnterpriseIdAsync(long enterpriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[CompanyPayrollSettings] WHERE EntrepriseID = @enterpriseId";
            var entity = await connection.QueryFirstOrDefaultAsync<CompanyPayrollSettingsEntity>(sql, new { enterpriseId });
            return entity != null ? MapToModel(entity) : null;
        }

        private static CompanyPayrollSettingsDataModel MapToModel(CompanyPayrollSettingsEntity e)
        {
            return new CompanyPayrollSettingsDataModel
            {
                CnapsComplementaryEmployerRate = e.CnapsComplementaryEmployerRate,
                CnapsComplementaryRate = e.CnapsComplementaryRate,
                CnapsEmployeeRate = e.CnapsEmployeeRate,
                CnapsEmployerRate = e.CnapsEmployerRate,
                EntrepriseID = e.EntrepriseID,
                IrsaDependentReduction = e.IrsaDependentReduction,
                IrsaMinimum = e.IrsaMinimum,
                MaxOvertimeHoursPerWeek = e.MaxOvertimeHoursPerWeek,
                OstieEmployeeRate = e.OstieEmployeeRate,
                OstieEmployerRate = e.OstieEmployerRate,
                OvertimeRateMultiplier = e.OvertimeRateMultiplier,
                RequireDoubleEntry = e.RequireDoubleEntry,
                SettingsID = e.SettingsID
            };
        }
    }
}
