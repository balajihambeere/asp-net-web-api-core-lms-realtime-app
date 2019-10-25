namespace LearningApiCore.Repositories
{
    using LearningApiCore.DataAccess;
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CourseRepository : ICourseRepository
    {
        private IMemoryCache _cache;
        private readonly AppDataContext _context;
        public CourseRepository(AppDataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<Course> Add(Course course)
        {
            course.CreatedOn = DateTime.Now;
            course.RowId = Guid.NewGuid();
            _context.Course.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Course.AnyAsync(e => e.CourseId == id);
        }

        public async Task<Course> Find(int id)
        {
            return await _context.Course.FindAsync(id);
        }

        public IEnumerable<Course> GetAll(bool isAuthenticated = false)
        {
            if (isAuthenticated)
            {
                var sourceCollection = _context.Course.Include(c => c.Category).Select(x => new Course
                {
                    CourseId = x.CourseId,
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    Slug = x.Slug,
                    IsActive = x.IsActive
                });
                return sourceCollection;
            }
            else
            {
                var sourceCollection = _context.Course.Where(x => x.IsActive).Select(x => new Course
                {
                    CourseId = x.CourseId,
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    Slug = x.Slug,
                    IsActive = x.IsActive
                });
                return sourceCollection;
            }
        }

        public async Task<Course> Update(Course course)
        {
            course.ModifiedOn = DateTime.Now;
            _context.Course.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public IEnumerable<Course> GetCourseByCategory(int categoryId, bool isAuthenticated = false)
        {
            if (isAuthenticated)
            {
                return _context.Course.Where(x => x.CategoryId == categoryId).Select(x => new Course
                {
                    CourseId = x.CourseId,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    CategoryId = x.CategoryId
                });
            }
            else
            {
                return _context.Course.Where(x => x.CategoryId == categoryId && x.IsActive).Select(x => new Course
                {
                    CourseId = x.CourseId,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    CategoryId = x.CategoryId
                });
            }
        }

        public IEnumerable<KeyValue> GetKeyValueByCategory(int categoryId)
        {
            var sourceCollection = _context.Course.Where(x => x.CategoryId == categoryId).Select(x => new KeyValue
            {
                Key = x.CourseId,
                Value = x.Name,
            });
            return sourceCollection;
        }

        public async Task<Course> FindBySlug(string name)
        {
            return await _context.Course.Where(x => x.Slug == name).FirstOrDefaultAsync();
        }

        public async Task<bool> Exists(string name)
        {
            return await _context.Course.AnyAsync(e => e.Name.Equals(name.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
