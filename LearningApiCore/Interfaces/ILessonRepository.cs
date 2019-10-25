namespace LearningApiCore.Interfaces
{
    using LearningApiCore.DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILessonRepository
    {
        IEnumerable<Lesson> GetAll();
        Task<Lesson> Find(int id);
        Task<Lesson> Add(Lesson lesson);
        Task<Lesson> Update(Lesson lesson);
        Task<bool> Exists(int id);
        Task<bool> Exists(string name);
        IEnumerable<Lesson> GetLessonByCourse(int courseId, bool isActive = false);
        IEnumerable<KeyValue> GetKeyValueByCourse(int courseId);
    }
}
