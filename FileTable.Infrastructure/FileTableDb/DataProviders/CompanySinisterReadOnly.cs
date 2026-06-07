using Dapper;
using Company.Sinister.Module.Data.Models;
using Company.Sinister.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanySinisterReadOnly : ICompanySinisterReadOnlyDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public CompanySinisterReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CompanySinisterDataModel?> ReadSinisterByIdAsync(long id)
        {
            using var connection = _dbContext.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE Id = @Id", new { Id = id });
            return entity == null ? null : MapToDataModel(entity);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE EntrepriseId = @EntrepriseId ORDER BY CreatedDate DESC",
                new { EntrepriseId = entrepriseId });
            return entities.Select(MapToDataModel);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByAssetTypeAsync(long entrepriseId, string assetType)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE EntrepriseId = @EntrepriseId AND AssetType = @AssetType ORDER BY CreatedDate DESC",
                new { EntrepriseId = entrepriseId, AssetType = assetType });
            return entities.Select(MapToDataModel);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByFleetAsync(long fleetId)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE EntrepriseFleetId = @FleetId ORDER BY CreatedDate DESC",
                new { FleetId = fleetId });
            return entities.Select(MapToDataModel);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByTransportationAsync(long transportationId)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE EntrepriseMerchandiseTransportationId = @TransportationId ORDER BY CreatedDate DESC",
                new { TransportationId = transportationId });
            return entities.Select(MapToDataModel);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByWarehouseAsync(long warehouseId)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE EntrepriseWarehouseId = @WarehouseId ORDER BY CreatedDate DESC",
                new { WarehouseId = warehouseId });
            return entities.Select(MapToDataModel);
        }

        public async Task<IEnumerable<CompanySinisterDataModel>> ReadSinistersByStatusAsync(long entrepriseId, string status)
        {
            using var connection = _dbContext.CreateConnection();
            var entities = await connection.QueryAsync<CompanySinisterEntity>(
                "SELECT * FROM [documentdb].[dbo].[EntrepriseSinister] WHERE EntrepriseId = @EntrepriseId AND Status = @Status ORDER BY CreatedDate DESC",
                new { EntrepriseId = entrepriseId, Status = status });
            return entities.Select(MapToDataModel);
        }

        protected static CompanySinisterDataModel MapToDataModel(CompanySinisterEntity e)
        {
            return new CompanySinisterDataModel
            {
                AssetType = e.AssetType,
                CreatedDate = e.CreatedDate,
                Description = e.Description,
                EntrepriseFleetId = e.EntrepriseFleetId,
                EntrepriseId = e.EntrepriseId,
                EntrepriseMerchandiseTransportationId = e.EntrepriseMerchandiseTransportationId,
                EntrepriseWarehouseId = e.EntrepriseWarehouseId,
                EstimatedAmount = e.EstimatedAmount,
                Id = e.Id,
                LastModifiedDate = e.LastModifiedDate,
                ResolvedAmount = e.ResolvedAmount,
                SinisterDate = e.SinisterDate,
                SinisterId = e.SinisterId,
                Status = e.Status
            };
        }
    }
}

