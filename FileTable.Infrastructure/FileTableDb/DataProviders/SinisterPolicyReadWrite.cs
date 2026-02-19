using Dapper;
using Sinister.Module.Data.Models;
using Sinister.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class SinisterPolicyReadWrite : SinisterPolicyReadOnly, ISinisterPolicyReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public SinisterPolicyReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> CreatePolicyAsync(SinisterPolicyDataModel model)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[Sinister]
                (PolicyNumber, EntrepriseId, InsurerName, InsurerContact, CoverageStartDate, CoverageEndDate, CoverageType, CoveredAssets, PremiumAmount, DeductibleAmount, PolicyLimits, IsActive, PolicyReferenceId, Notes, CreatedDate, LastModifiedDate)
                VALUES
                (@PolicyNumber, @EntrepriseId, @InsurerName, @InsurerContact, @CoverageStartDate, @CoverageEndDate, @CoverageType, @CoveredAssets, @PremiumAmount, @DeductibleAmount, @PolicyLimits, @IsActive, @PolicyReferenceId, @Notes, @CreatedDate, @LastModifiedDate);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            model.CreatedDate = model.CreatedDate == default ? DateTime.UtcNow : model.CreatedDate;
            model.LastModifiedDate = model.LastModifiedDate == default ? DateTime.UtcNow : model.LastModifiedDate;

            return await connection.ExecuteScalarAsync<long>(sql, model);
        }

        public async Task UpdatePolicyAsync(SinisterPolicyDataModel model)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[Sinister]
                SET InsurerName = @InsurerName,
                    InsurerContact = @InsurerContact,
                    CoverageStartDate = @CoverageStartDate,
                    CoverageEndDate = @CoverageEndDate,
                    CoverageType = @CoverageType,
                    CoveredAssets = @CoveredAssets,
                    PremiumAmount = @PremiumAmount,
                    DeductibleAmount = @DeductibleAmount,
                    PolicyLimits = @PolicyLimits,
                    IsActive = @IsActive,
                    PolicyReferenceId = @PolicyReferenceId,
                    Notes = @Notes,
                    LastModifiedDate = @LastModifiedDate
                WHERE Id = @Id";

            model.LastModifiedDate = DateTime.UtcNow;
            await connection.ExecuteAsync(sql, model);
        }

        public async Task DeletePolicyAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[Sinister] WHERE Id = @id";
            await connection.ExecuteAsync(sql, new { id });
        }
    }
}
