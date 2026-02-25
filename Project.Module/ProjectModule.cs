using Project.Module.Business;
using Project.Module.Data.Providers;

namespace Project.Module
{
    public class ProjectModule : IProjectModule
    {
        private readonly IReadProject _projectReadOnly;

        public ProjectModule(IReadProject projectReadOnly)
        {
            _projectReadOnly = projectReadOnly;
        }

        public async Task<List<ProjectBusinessModel>> GetActiveProjectsAsync()
        {
            var items = await _projectReadOnly.ReadActiveProjectsAsync();
            return items.Select(ProjectBusinessModel.FromDataModel).ToList();
        }
    }
}
