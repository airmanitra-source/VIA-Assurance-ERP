using Project.Module.Data.Models;

namespace Project.Module.Data.Providers
{
    public interface IProjectReadOnly
    {
        Task<List<ProjectDataModel>> ReadActiveProjectsAsync();
        Task<ProjectDataModel?> ReadProjectByEmployeeIdAsync(long employeeId);
    }
}
