using EmployeePayroll.Module.Data.Models;
using PaySlip.Module.Data.Models;

namespace PaySlip.Module.Data.Providers
{
    public interface IPaySlipReadWriteDataProvider
    {
        Task UpdatePaySlipAsync(EmployeePayrollDataModel payroll, List<PaySlipLineDataModel> lines);
    }
}

