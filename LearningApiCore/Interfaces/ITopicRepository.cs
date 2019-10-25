namespace LearningApiCore.Interfaces
{
    using LearningApiCore.DataAccess.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITopicRepository
    {
        IEnumerable<Topic> GetAll();
        Task<Topic> Find(int id);
        Task<Topic> FindBySlug(string slug);
        Task<Topic> Add(Topic topic);
        Task<Topic> Update(Topic topic);
        Task<bool> Exists(int id);
        Task<bool> Exists(string name, int lessonId);
        IEnumerable<Topic> GetTopicByLesson(int lessonId);
    }
}
