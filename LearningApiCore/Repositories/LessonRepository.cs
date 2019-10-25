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

    public class LessonRepository : ILessonRepository
    {
        private readonly AppDataContext _context;
        public LessonRepository(AppDataContext context)
        {
            _context = context;
        }

        public async Task<Lesson> Add(Lesson lesson)
        {
            lesson.CreatedOn = DateTime.Now;
            lesson.RowId = Guid.NewGuid();
            _context.Lesson.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Lesson.AnyAsync(e => e.LessonId == id);
        }

        public async Task<bool> Exists(string name)
        {
            return await _context.Lesson.AnyAsync(e => e.Name.Equals(name.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Lesson> Find(int id)
        {
            return await _context.Lesson.Include(x => x.Course).FirstOrDefaultAsync(x => x.LessonId == id);
        }

        public IEnumerable<Lesson> GetAll()
        {
            var sourceCollection = _context.Lesson.Select(x => new Lesson
            {
                LessonId = x.LessonId,
                Name = x.Name,
                SortOrder = x.SortOrder,
                IsActive = x.IsActive,
                CourseId = x.CourseId
            });
            return sourceCollection;
        }

        public IEnumerable<KeyValue> GetKeyValueByCourse(int courseId)
        {
            var sourceCollection = _context.Lesson.Where(x => x.CourseId == courseId).Select(x => new KeyValue
            {
                Key = x.LessonId,
                Value = x.Name
            });
            return sourceCollection;
        }

        public IEnumerable<Lesson> GetLessonByCourse(int courseId, bool isActive = false)
        {
            if (isActive)
            {
                return _context.Lesson.Where(x => x.CourseId == courseId && x.IsActive == isActive).Select(x => new Lesson
                {
                    LessonId = x.LessonId,
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,
                });
            }
            else
            {
                return _context.Lesson.Where(x => x.CourseId == courseId).Select(x => new Lesson
                {
                    LessonId = x.LessonId,
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive,
                });
            }
        }

        public async Task<Lesson> Update(Lesson lesson)
        {
            lesson.ModifiedOn = DateTime.Now;
            _context.Lesson.Update(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }
    }
}
