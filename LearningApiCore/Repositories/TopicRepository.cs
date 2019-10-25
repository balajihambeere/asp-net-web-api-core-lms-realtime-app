namespace LearningApiCore.Repositories
{
    using LearningApiCore.DataAccess;
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TopicRepository : ITopicRepository
    {
        private readonly AppDataContext _context;
        public TopicRepository(AppDataContext context)
        {
            _context = context;
        }

        public async Task<Topic> Add(Topic topic)
        {
            topic.CreatedOn = DateTime.Now;
            topic.RowId = Guid.NewGuid();
            _context.Topic.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Topic.AnyAsync(e => e.TopicId == id);
        }

        public async Task<bool> Exists(string name, int lessonId)
        {
            return await _context.Topic.AnyAsync(e => e.Title.Equals(name.ToString(), StringComparison.OrdinalIgnoreCase) && e.LessonId == lessonId);
        }

        public async Task<Topic> Find(int id)
        {
            return await _context.Topic.Include(x => x.Lesson).SingleOrDefaultAsync(y => y.TopicId == id);
        }

        public async Task<Topic> FindBySlug(string slug)
        {
            return await _context.Topic.Include(x => x.Lesson).SingleOrDefaultAsync(y => y.Slug == slug);
        }

        public IEnumerable<Topic> GetAll()
        {
            var sourceCollection = _context.Topic.Select(x => new Topic
            {
                TopicId = x.TopicId,
                LessonId = x.LessonId,
                Title = x.Title,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive
            });
            return sourceCollection;
        }

        public IEnumerable<Topic> GetTopicByLesson(int lessonId)
        {
            var sourceCollection = _context.Topic.Where(x => x.LessonId == lessonId).Select(x => new Topic
            {
                TopicId = x.TopicId,
                Title = x.Title,
                IsActive = x.IsActive,
            });
            return sourceCollection;
        }

        public async Task<Topic> Update(Topic topic)
        {
            topic.ModifiedOn = DateTime.Now;
            _context.Topic.Update(topic);
            await _context.SaveChangesAsync();
            return topic;
        }
    }
}
