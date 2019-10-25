namespace LearningApiCore.Interfaces
{
    using LearningApiCore.DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICourseRepository
    {
        IEnumerable<Course> GetAll(bool isAuthenticated = false);
        Task<Course> Find(int id);
        Task<Course> FindBySlug(string name);
        Task<Course> Add(Course course);
        Task<Course> Update(Course course);
        Task<bool> Exists(int id);
        Task<bool> Exists(string name);
        IEnumerable<Course> GetCourseByCategory(int categoryId, bool isAuthenticated = false);
        IEnumerable<KeyValue> GetKeyValueByCategory(int categoryId);
    }
}
