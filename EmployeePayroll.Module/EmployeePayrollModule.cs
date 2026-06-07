using EmployeePayroll.Module.Business;
using EmployeePayroll.Module.Data.Models;
using EmployeePayroll.Module.Data.Providers;

namespace EmployeePayroll.Module
{
    public class EmployeePayrollModule : IEmployeePayrollModule
    {
        private readonly IPayrollPeriodReadOnlyDataProvider _periodReadOnly;
        private readonly IPayrollPeriodReadWriteDataProvider _periodReadWrite;
        private readonly IReadEmployeePayrollDataProvider _payrollReadOnly;

        public EmployeePayrollModule(
            IPayrollPeriodReadOnlyDataProvider periodReadOnly,
            IPayrollPeriodReadWriteDataProvider periodReadWrite,
            IReadEmployeePayrollDataProvider payrollReadOnly)
        {
            _periodReadOnly = periodReadOnly;
            _periodReadWrite = periodReadWrite;
            _payrollReadOnly = payrollReadOnly;
        }

        public async Task<int> AddPeriodAsync(long enterpriseId, DateTime periodStart, DateTime periodEnd)
        {
            var dataModel = new PayrollPeriodDataModel
            {
                EntrepriseID = enterpriseId,
                PeriodEnd = periodEnd,
                PeriodStart = periodStart,
                Status = "Draft"
            };
            return await _periodReadWrite.CreatePeriodAsync(dataModel);
        }

        public async Task<List<EmployeePayrollBusinessModel>> GetLastMonthsPayrollAsync(long employeeId, int months = 3)
        {
            var items = await _payrollReadOnly.ReadLastMonthsPayrollAsync(employeeId, months);
            return items.Select(EmployeePayrollBusinessModel.FromDataModel).ToList();
        }

        public async Task<List<PayrollPeriodBusinessModel>> GetPeriodsByEnterpriseAsync(long enterpriseId)
        {
            var items = await _periodReadOnly.ReadByEnterpriseIdAsync(enterpriseId);
            return items.Select(PayrollPeriodBusinessModel.FromDataModel).ToList();
        }

        public async Task SetPeriodStatusAsync(int periodId, string status)
        {
            await _periodReadWrite.UpdatePeriodStatusAsync(periodId, status);
        }
    }
}

