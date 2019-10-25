namespace LearningApiCore.Interfaces
{
    using LearningApiCore.DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Task<Category> Find(int id);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task<bool> Exists(int id);
        Task<bool> Exists(string name);
        IEnumerable<KeyValue> GetkeyValue(bool isAuthenticated = false);
    }
}
