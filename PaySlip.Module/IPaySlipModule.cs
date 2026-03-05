using PaySlip.Module.Business;

namespace PaySlip.Module
{
    public interface IPaySlipModule
    {
        Task<int> AddModificationRequestAsync(PaySlipModificationRequestBusinessModel request);
        Task<int> AddPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear);
        Task<int> AddSecondEntryAsync(PaySlipSecondEntryBusinessModel entry);
        Task<PaySlipModificationRequestBusinessModel?> GetModificationRequestAsync(long employeeId, int periodId);
        Task<List<PaySlipModificationRequestBusinessModel>> GetModificationRequestsByPeriodAsync(int periodId);
        Task<PaySlipBusinessModel> GetPaySlipAsync(long employeeId, int periodId, long enterpriseId);
        Task<PaySlipBusinessModel?> GetSavedPaySlipAsync(long employeeId, int periodId);
        Task<PaySlipSecondEntryBusinessModel?> GetSecondEntryAsync(long employeeId, int periodId);
        Task RemovePayrollAsync(int payrollId);
        Task RemoveSecondEntryAsync(long employeeId, int periodId);
        Task SetModificationRequestStatusAsync(int requestId, string status);
        Task SetPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear);
        Task SetRecalculateDraftPaySlipsForEmployeeAsync(long employeeId, long enterpriseId);
        Task SetRecalculateDraftPaySlipsForEmployeeAsync(long employeeId, long enterpriseId, decimal? updatedSalary = null);
    }
}
