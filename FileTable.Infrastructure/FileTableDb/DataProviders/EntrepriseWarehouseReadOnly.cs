using Dapper;
using Company.Warehouse.Module.Data.Models;
using Company.Warehouse.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EntrepriseWarehouseReadOnly : IEntrepriseWarehouseReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public EntrepriseWarehouseReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EntrepriseWarehouseDataModel?> GetWarehouseByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseWarehouse] WHERE Id = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseWarehouseEntity>(sql, new { Id = id });
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<IEnumerable<EntrepriseWarehouseDataModel>> GetWarehousesByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseWarehouse] WHERE EntrepriseId = @EntrepriseId";
            var entities = await connection.QueryAsync<EntrepriseWarehouseEntity>(sql, new { EntrepriseId = entrepriseId });
            return entities.Select(MapToModel);
        }

        public async Task<EntrepriseWarehouseMaterialDataModel?> GetMaterialByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseWarehouseMaterials] WHERE Id = @Id";
            var entity = await connection.QueryFirstOrDefaultAsync<EntrepriseWarehouseMaterialEntity>(sql, new { Id = id });
            return entity != null ? MapToMaterialModel(entity) : null;
        }

        public async Task<IEnumerable<EntrepriseWarehouseMaterialDataModel>> GetMaterialsByWarehouseIdAsync(long warehouseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT * FROM [documentdb].[dbo].[EntrepriseWarehouseMaterials] WHERE WarehouseId = @WarehouseId";
            var entities = await connection.QueryAsync<EntrepriseWarehouseMaterialEntity>(sql, new { WarehouseId = warehouseId });
            return entities.Select(MapToMaterialModel);
        }

        protected EntrepriseWarehouseDataModel MapToModel(EntrepriseWarehouseEntity entity)
        {
            return new EntrepriseWarehouseDataModel
            {
                Id = entity.Id,
                EntrepriseId = entity.EntrepriseId,
                Name = entity.Name,
                SizeM2 = entity.SizeM2,
                ContentsDescription = entity.ContentsDescription,
                Address = entity.Address,
                WantsInsurance = entity.WantsInsurance,
                IsInsured = entity.IsInsured
            };
        }

        protected EntrepriseWarehouseMaterialDataModel MapToMaterialModel(EntrepriseWarehouseMaterialEntity entity)
        {
            return new EntrepriseWarehouseMaterialDataModel
            {
                Id = entity.Id,
                WarehouseId = entity.WarehouseId,
                Description = entity.Description,
                ApproximateValue = entity.ApproximateValue,
                WantsInsurance = entity.WantsInsurance,
                IsInsured = entity.IsInsured
            };
        }
    }
}
