using Employee.Module.Business;

namespace Employee.Module
{
    public interface IPayrollModule
    {
        Task<int> AddPeriodAsync(long enterpriseId, DateTime periodStart, DateTime periodEnd);
        Task<PaySlipBusinessModel> GetPaySlipAsync(long employeeId, int periodId, long enterpriseId);
        Task<List<PayrollPeriodBusinessModel>> GetPeriodsByEnterpriseAsync(long enterpriseId);
        Task<CompanyPayrollSettingsBusinessModel> GetSettingsAsync(long enterpriseId);
        Task<int> AddPaySlipAsync(PaySlipBusinessModel paySlip, DateTime paymentDate, int periodMonth, int periodYear);
        Task SetPeriodStatusAsync(int periodId, string status);
    }
}
