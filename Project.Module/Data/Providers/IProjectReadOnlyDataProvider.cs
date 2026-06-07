using Project.Module.Data.Models;

namespace Project.Module.Data.Providers
{
    public interface IProjectReadOnlyDataProvider
    {
        Task<List<ProjectDataModel>> ReadActiveProjectsAsync();
        Task<ProjectDataModel?> ReadProjectByEmployeeIdAsync(long employeeId);
    }
}

