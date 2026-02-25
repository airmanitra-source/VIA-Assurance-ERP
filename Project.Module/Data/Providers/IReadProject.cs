using Project.Module.Data.Models;

namespace Project.Module.Data.Providers
{
    public interface IReadProject
    {
        Task<List<ProjectDataModel>> ReadActiveProjectsAsync();
    }
}
