using Company.Sinister.Module.Business;
using Company.Sinister.Module.Data.Providers;
using CompanySinisterDocument.Module;
using FileTable.Infrastructure.Abstractions;

namespace Company.Sinister.Module
{
    public class CompanySinisterModule : ICompanySinisterModule
    {
        private readonly ICompanySinisterDocumentModule _documentModule;
        private readonly ICompanySinisterReadOnly _readOnlyProvider;
        private readonly ICompanySinisterReadWrite _readWriteProvider;
        private readonly ITransactionDetector _transactionDetector;
        private readonly ITransactionHandler _transactionHandler;

        private ICompanySinisterReadOnly ReadProvider =>
           _transactionDetector.IsTransactionActive() ? _readWriteProvider : _readOnlyProvider;

        public CompanySinisterModule(
            ICompanySinisterReadOnly readOnlyProvider,
            ICompanySinisterReadWrite readWriteProvider,
            ICompanySinisterDocumentModule documentModule,
            ITransactionDetector transactionDetector,
            ITransactionHandler transactionHandler)
        {
            _documentModule = documentModule;
            _readOnlyProvider = readOnlyProvider;
            _readWriteProvider = readWriteProvider;
            _transactionDetector = transactionDetector;
            _transactionHandler = transactionHandler;
        }

        public async Task<long> AddSinisterAsync(CompanySinisterBusinessModel sinister, IReadOnlyList<(string FileName, byte[] FileContent, string TypeDocument)> documents)
        {
            return await _transactionHandler.ExecuteInTransactionAsync(async () =>
            {
                var dataModel = sinister.ToDataModel();
                dataModel.CreatedDate = DateTime.UtcNow;
                dataModel.LastModifiedDate = DateTime.UtcNow;
                var sinisterId = await _readWriteProvider.AddSinisterAsync(dataModel);

                foreach (var doc in documents)
                {
                    await _documentModule.AddDocumentAsync(sinister.EntrepriseId, sinisterId, doc.FileName, doc.FileContent, doc.TypeDocument);
                }

                return sinisterId;
            });
        }

        public async Task DeleteSinisterAsync(long id)
        {
            await _readWriteProvider.DeleteSinisterAsync(id);
        }

        public async Task<IEnumerable<CompanySinisterBusinessModel>> GetCompanySinistersAsync(long entrepriseId)
        {
            var items = await ReadProvider.ReadSinistersByEntrepriseIdAsync(entrepriseId);
            return items.Select(CompanySinisterBusinessModel.From);
        }

        public async Task<CompanySinisterBusinessModel?> GetSinisterByIdAsync(long id)
        {
            var dataModel = await ReadProvider.ReadSinisterByIdAsync(id);
            return dataModel != null ? CompanySinisterBusinessModel.From(dataModel) : null;
        }

        public async Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByAssetTypeAsync(long entrepriseId, string assetType)
        {
            var items = await ReadProvider.ReadSinistersByAssetTypeAsync(entrepriseId, assetType);
            return items.Select(CompanySinisterBusinessModel.From);
        }

        public async Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByFleetAsync(long fleetId)
        {
            var items = await ReadProvider.ReadSinistersByFleetAsync(fleetId);
            return items.Select(CompanySinisterBusinessModel.From);
        }

        public async Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByStatusAsync(long entrepriseId, string status)
        {
            var items = await ReadProvider.ReadSinistersByStatusAsync(entrepriseId, status);
            return items.Select(CompanySinisterBusinessModel.From);
        }

        public async Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByTransportationAsync(long transportationId)
        {
            var items = await ReadProvider.ReadSinistersByTransportationAsync(transportationId);
            return items.Select(CompanySinisterBusinessModel.From);
        }

        public async Task<IEnumerable<CompanySinisterBusinessModel>> GetSinistersByWarehouseAsync(long warehouseId)
        {
            var items = await ReadProvider.ReadSinistersByWarehouseAsync(warehouseId);
            return items.Select(CompanySinisterBusinessModel.From);
        }

        public async Task SetSinisterAsync(CompanySinisterBusinessModel sinister)
        {
            var existing = await _readWriteProvider.ReadSinisterByIdAsync(sinister.Id);
            if (existing != null && string.Equals(existing.Status, "Resolved", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Approved sinister cannot be modified.");
            }

            if (string.Equals(sinister.Status, "Resolved", StringComparison.OrdinalIgnoreCase)
                && (!sinister.ResolvedAmount.HasValue || sinister.ResolvedAmount.Value <= 0))
            {
                throw new InvalidOperationException("Resolved amount is required to approve a sinister.");
            }

            var dataModel = sinister.ToDataModel();
            dataModel.LastModifiedDate = DateTime.UtcNow;
            await _readWriteProvider.UpdateSinisterAsync(dataModel);
        }
    }
}
