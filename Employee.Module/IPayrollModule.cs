using Employee.Module.Business;

namespace Employee.Module
{
    public interface IPayrollModule
    {
        Task<int> AddModificationRequestAsync(PaySlipModificationRequestBusinessModel request);
        Task<int> AddPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear);
        Task<int> AddPeriodAsync(long enterpriseId, DateTime periodStart, DateTime periodEnd);
        Task<PaySlipModificationRequestBusinessModel?> GetModificationRequestAsync(long employeeId, int periodId);
        Task<List<PaySlipModificationRequestBusinessModel>> GetModificationRequestsByPeriodAsync(int periodId);
        Task<PaySlipBusinessModel> GetPaySlipAsync(long employeeId, int periodId, long enterpriseId);
        Task<List<PayrollPeriodBusinessModel>> GetPeriodsByEnterpriseAsync(long enterpriseId);
        Task<PaySlipBusinessModel?> GetSavedPaySlipAsync(long employeeId, int periodId);
        Task<CompanyPayrollSettingsBusinessModel> GetSettingsAsync(long enterpriseId);
        Task SetModificationRequestStatusAsync(int requestId, string status);
        Task SetPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear);
        Task SetPeriodStatusAsync(int periodId, string status);
    }
}
