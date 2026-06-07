using CompanyPayroll.Module.Data.Models;
using CompanyPayroll.Module.Data.Providers;
using Dapper;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanyPayrollSettingsReadWrite : ICompanyPayrollSettingsReadWriteDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public CompanyPayrollSettingsReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateSettingsAsync(CompanyPayrollSettingsDataModel settings)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[CompanyPayrollSettings]
                (EntrepriseID, CnapsEmployeeRate, CnapsComplementaryRate, OstieEmployeeRate,
                 CnapsEmployerRate, CnapsComplementaryEmployerRate, OstieEmployerRate,
                 MaxOvertimeHoursPerWeek, OvertimeRateMultiplier, IrsaMinimum, IrsaDependentReduction, RequireDoubleEntry)
                VALUES
                (@EntrepriseID, @CnapsEmployeeRate, @CnapsComplementaryRate, @OstieEmployeeRate,
                 @CnapsEmployerRate, @CnapsComplementaryEmployerRate, @OstieEmployerRate,
                 @MaxOvertimeHoursPerWeek, @OvertimeRateMultiplier, @IrsaMinimum, @IrsaDependentReduction, @RequireDoubleEntry);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(sql, settings);
        }

        public async Task UpdateSettingsAsync(CompanyPayrollSettingsDataModel settings)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[CompanyPayrollSettings]
                SET CnapsEmployeeRate = @CnapsEmployeeRate,
                    CnapsComplementaryRate = @CnapsComplementaryRate,
                    OstieEmployeeRate = @OstieEmployeeRate,
                    CnapsEmployerRate = @CnapsEmployerRate,
                    CnapsComplementaryEmployerRate = @CnapsComplementaryEmployerRate,
                    OstieEmployerRate = @OstieEmployerRate,
                    MaxOvertimeHoursPerWeek = @MaxOvertimeHoursPerWeek,
                    OvertimeRateMultiplier = @OvertimeRateMultiplier,
                    IrsaMinimum = @IrsaMinimum,
                    IrsaDependentReduction = @IrsaDependentReduction,
                    RequireDoubleEntry = @RequireDoubleEntry,
                    ModifiedDate = GETDATE()
                WHERE SettingsID = @SettingsID";
            await connection.ExecuteAsync(sql, settings);
        }
    }
}

