using Dapper;
using Company.Warehouse.Module.Data.Models;
using Company.Warehouse.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseWarehouseReadWrite : EntrepriseWarehouseReadOnly, IEntrepriseWarehouseReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseWarehouseReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> AddWarehouseAsync(EntrepriseWarehouseDataModel warehouse)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EntrepriseWarehouse] (EntrepriseId, Name, SizeM2, ContentsDescription, Address, WantsInsurance, IsInsured, PolicyNumber)
                VALUES (@EntrepriseId, @Name, @SizeM2, @ContentsDescription, @Address, @WantsInsurance, @IsInsured, @PolicyNumber);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, warehouse);
        }

        public async Task UpdateWarehouseAsync(EntrepriseWarehouseDataModel warehouse)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseWarehouse]
                SET Name = @Name,
                    SizeM2 = @SizeM2,
                    ContentsDescription = @ContentsDescription,
                    Address = @Address,
                    WantsInsurance = @WantsInsurance,
                    IsInsured = @IsInsured,
                    PolicyNumber = @PolicyNumber
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, warehouse);
        }

        public async Task DeleteWarehouseAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[EntrepriseWarehouse] WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<long> AddMaterialAsync(EntrepriseWarehouseMaterialDataModel material)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EntrepriseWarehouseMaterials] (WarehouseId, Description, ApproximateValue, WantsInsurance, IsInsured)
                VALUES (@WarehouseId, @Description, @ApproximateValue, @WantsInsurance, @IsInsured);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            return await connection.ExecuteScalarAsync<long>(sql, material);
        }

        public async Task UpdateMaterialAsync(EntrepriseWarehouseMaterialDataModel material)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[EntrepriseWarehouseMaterials]
                SET Description = @Description,
                    ApproximateValue = @ApproximateValue,
                    WantsInsurance = @WantsInsurance,
                    IsInsured = @IsInsured
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, material);
        }

        public async Task DeleteMaterialAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "DELETE FROM [documentdb].[dbo].[EntrepriseWarehouseMaterials] WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
