namespace LearningApiCore.Interfaces
{
    using LearningApiCore.DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILanguageRepository
    {
        IEnumerable<Language> GetAll();
        Task<Language> Find(int id);
        Task<Language> Add(Language language);
        Task<Language> Update(Language language);
        Task<bool> Exists(int id);
        Task<bool> Exists(string name);
        IEnumerable<KeyValue> GetkeyValue();
    }
}
