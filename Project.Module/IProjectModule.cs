using Project.Module.Business;

namespace Project.Module
{
    public interface IProjectModule
    {
        Task<List<ProjectBusinessModel>> GetActiveProjectsAsync();
    }
}
