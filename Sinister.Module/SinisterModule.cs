using Sinister.Module.Business;
using Sinister.Module.Data.Models;
using Sinister.Module.Data.Providers;

namespace Sinister.Module
{
    public class SinisterModule : ISinisterModule
    {
        private readonly ISinisterPolicyReadWriteDataProvider _policyReadWrite;
        private readonly ISinisterPolicyReadOnlyDataProvider _policyReadOnly;

        public SinisterModule(ISinisterPolicyReadWriteDataProvider policyReadWrite, ISinisterPolicyReadOnlyDataProvider policyReadOnly)
        {
            _policyReadWrite = policyReadWrite;
            _policyReadOnly = policyReadOnly;
        }

        public async Task<long> AddPolicyAsync(SinisterPolicyBusinessModel policy)
        {
            var data = MapToDataModel(policy);
            return await _policyReadWrite.CreatePolicyAsync(data);
        }

        public async Task<IEnumerable<SinisterPolicyBusinessModel>> GetPoliciesByEntrepriseIdAsync(long entrepriseId)
        {
            var items = await _policyReadWrite.ReadPoliciesByEntrepriseIdAsync(entrepriseId);
            return items.Select(MapToBusinessModel);
        }

        public async Task<SinisterPolicyBusinessModel?> GetPolicyByIdAsync(long id)
        {
            var item = await _policyReadOnly.ReadPolicyByIdAsync(id);
            return item == null ? null : MapToBusinessModel(item);
        }

        public async Task UpdatePolicyAsync(SinisterPolicyBusinessModel policy)
        {
            var data = MapToDataModel(policy);
            await _policyReadWrite.UpdatePolicyAsync(data);
        }

        public async Task RemovePolicyAsync(long id)
        {
            await _policyReadWrite.DeletePolicyAsync(id);
        }

        private static SinisterPolicyDataModel MapToDataModel(SinisterPolicyBusinessModel b)
        {
            return new SinisterPolicyDataModel
            {
                Id = b.Id,
                PolicyNumber = b.PolicyNumber,
                EntrepriseId = b.EntrepriseId,
                InsurerName = b.InsurerName,
                InsurerContact = b.InsurerContact,
                CoverageStartDate = b.CoverageStartDate,
                CoverageEndDate = b.CoverageEndDate,
                CoverageType = b.CoverageType,
                CoveredAssets = b.CoveredAssets,
                PremiumAmount = b.PremiumAmount,
                DeductibleAmount = b.DeductibleAmount,
                PolicyLimits = b.PolicyLimits,
                IsActive = b.IsActive,
                PolicyReferenceId = b.PolicyReferenceId,
                Notes = b.Notes,
                CreatedDate = b.CreatedDate,
                LastModifiedDate = b.LastModifiedDate
            };
        }

        private static SinisterPolicyBusinessModel MapToBusinessModel(SinisterPolicyDataModel d)
        {
            return new SinisterPolicyBusinessModel
            {
                Id = d.Id,
                PolicyNumber = d.PolicyNumber,
                EntrepriseId = d.EntrepriseId,
                InsurerName = d.InsurerName,
                InsurerContact = d.InsurerContact,
                CoverageStartDate = d.CoverageStartDate,
                CoverageEndDate = d.CoverageEndDate,
                CoverageType = d.CoverageType,
                CoveredAssets = d.CoveredAssets,
                PremiumAmount = d.PremiumAmount,
                DeductibleAmount = d.DeductibleAmount,
                PolicyLimits = d.PolicyLimits,
                IsActive = d.IsActive,
                PolicyReferenceId = d.PolicyReferenceId,
                Notes = d.Notes,
                CreatedDate = d.CreatedDate,
                LastModifiedDate = d.LastModifiedDate
            };
        }
    }
}

