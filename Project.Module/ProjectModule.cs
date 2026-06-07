using Project.Module.Business;
using Project.Module.Data.Providers;

namespace Project.Module
{
    public class ProjectModule : IProjectModule
    {
        private readonly IProjectReadOnlyDataProvider _projectReadOnly;

        public ProjectModule(IProjectReadOnlyDataProvider projectReadOnly)
        {
            _projectReadOnly = projectReadOnly;
        }

        public async Task<List<ProjectBusinessModel>> GetActiveProjectsAsync()
        {
            var items = await _projectReadOnly.ReadActiveProjectsAsync();
            return items.Select(ProjectBusinessModel.FromDataModel).ToList();
        }

        public async Task<ProjectBusinessModel?> GetProjectByEmployeeIdAsync(long employeeId)
        {
            var project = await _projectReadOnly.ReadProjectByEmployeeIdAsync(employeeId);
            return project != null ? ProjectBusinessModel.FromDataModel(project) : null;
        }
    }
}

