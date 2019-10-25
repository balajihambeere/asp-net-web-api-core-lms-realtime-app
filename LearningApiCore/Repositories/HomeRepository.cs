namespace LearningApiCore.Repositories
{
    using LearningApiCore.DataAccess;
    using LearningApiCore.Interfaces;
    using LearningApiCore.ViewModels;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;

    public class HomeRepository : IHomeRepository
    {

        private readonly AppDataContext _context;
        public HomeRepository(AppDataContext context)
        {
            _context = context;
        }

        public CourseIndexViewModel GetCourseIndex(string slug, bool isAuthenticated = false)
        {
            var target = new CourseIndexViewModel { };
            if (isAuthenticated)
            {
                var result = _context.Course.Include(x => x.Lessons).FirstOrDefault(x => x.Slug == slug);
                if (result != null)
                {
                    var category = _context.Category.Find(result.CategoryId);
                    if (category != null)
                    {
                        target.Category = category.Name;
                        target.CategorySlug = category.Slug;
                    }
                    target.CourseId = result.CourseId;
                    target.Name = result.Name;
                    target.Slug = result.Slug;

                    foreach (var item in result.Lessons)
                    {
                        var topics = _context.Topic.Where(x => x.LessonId == item.LessonId);
                        List<TopicIndexViewModel> newTopics = new List<TopicIndexViewModel>();
                        foreach (var subItem in topics)
                        {
                            newTopics.Add(new TopicIndexViewModel
                            {
                                Name = subItem.Title,
                                Slug = subItem.Slug,
                                TopicId = subItem.TopicId
                            });
                        }
                        target.Lessons.Add(new LessonIndexViewModel
                        {
                            LessonId = item.LessonId,
                            Name = item.Name,
                            Slug = item.Slug,
                            Topics = newTopics
                        });
                    }
                }
            }
            else
            {
                var result = _context.Course.Include(x => x.Lessons).FirstOrDefault(x => x.Slug == slug && x.IsActive);
                if (result != null)
                {
                    var category = _context.Category.Find(result.CategoryId);
                    if (category != null)
                    {
                        target.Category = category.Name;
                        target.CategorySlug = category.Slug;
                    }
                    target.CourseId = result.CourseId;
                    target.Name = result.Name;
                    target.Slug = result.Slug;

                    foreach (var item in result.Lessons)
                    {
                        var topics = _context.Topic.Where(x => x.LessonId == item.LessonId && x.IsActive);
                        List<TopicIndexViewModel> newTopics = new List<TopicIndexViewModel>();
                        foreach (var subItem in topics)
                        {
                            newTopics.Add(new TopicIndexViewModel
                            {
                                Name = subItem.Title,
                                Slug = subItem.Slug,
                                TopicId = subItem.TopicId
                            });
                        }
                        target.Lessons.Add(new LessonIndexViewModel
                        {
                            LessonId = item.LessonId,
                            Name = item.Name,
                            Slug = item.Slug,
                            Topics = newTopics
                        });
                    }
                }
            }

            return target;
        }

        public IEnumerable<CourseListViewModel> GetCoursesByCategorName(string name, bool isAuthenticated = false)
        {
            if (isAuthenticated)
            {
                var category = _context.Category.Where(x => x.Slug == name).FirstOrDefault();
                if (category == null)
                {
                    return null;
                }
                var sourceCollection = _context.Course.Where(x => x.CategoryId == category.CategoryId).Select(x => new CourseListViewModel
                {
                    CourseId = x.CourseId,
                    Name = x.Name,
                    Slug = x.Slug,
                    CategoryId = x.CategoryId,
                    Category = category.Name,
                    CategorySlug = category.Slug
                });
                return sourceCollection;
            }
            else
            {
                var category = _context.Category.Where(x => x.Slug == name).FirstOrDefault();
                if (category == null)
                {
                    return null;
                }
                var sourceCollection = _context.Course.Where(x => x.CategoryId == category.CategoryId && x.IsActive).Select(x => new CourseListViewModel
                {
                    CourseId = x.CourseId,
                    Name = x.Name,
                    Slug = x.Slug,
                    CategoryId = x.CategoryId,
                    Category = category.Name,
                    CategorySlug = category.Slug
                });
                return sourceCollection;
            }
        }

        public CourseIndexViewModel GetCourseIndex(int id, bool isAuthenticated = false)
        {
                var target = new CourseIndexViewModel { };
                if (isAuthenticated)
                {
                    var result = _context.Course.Include(x => x.Lessons).FirstOrDefault(x => x.CourseId == id);
                    if (result != null)
                    {
                        var category = _context.Category.Find(result.CategoryId);
                        if (category != null)
                        {
                            target.Category = category.Name;
                            target.CategorySlug = category.Slug;
                        }
                        target.CourseId = result.CourseId;
                        target.Name = result.Name;
                        target.Slug = result.Slug;

                        foreach (var item in result.Lessons)
                        {
                            var topics = _context.Topic.Where(x => x.LessonId == item.LessonId);
                            List<TopicIndexViewModel> newTopics = new List<TopicIndexViewModel>();
                            foreach (var subItem in topics)
                            {
                                newTopics.Add(new TopicIndexViewModel
                                {
                                    Name = subItem.Title,
                                    Slug = subItem.Slug,
                                    TopicId = subItem.TopicId
                                });
                            }
                            target.Lessons.Add(new LessonIndexViewModel
                            {
                                LessonId = item.LessonId,
                                Name = item.Name,
                                Slug = item.Slug,
                                Topics = newTopics
                            });
                        }
                    }
                }
                else
                {

                    var result = _context.Course.Include(x => x.Lessons).FirstOrDefault(x => x.CourseId == id && x.IsActive);
                    if (result != null)
                    {
                        var category = _context.Category.Find(result.CategoryId);
                        if (category != null)
                        {
                            target.Category = category.Name;
                            target.CategorySlug = category.Slug;
                        }
                        target.CourseId = result.CourseId;
                        target.Name = result.Name;
                        target.Slug = result.Slug;

                        foreach (var item in result.Lessons)
                        {
                            var topics = _context.Topic.Where(x => x.LessonId == item.LessonId && x.IsActive);
                            List<TopicIndexViewModel> newTopics = new List<TopicIndexViewModel>();
                            foreach (var subItem in topics)
                            {
                                newTopics.Add(new TopicIndexViewModel
                                {
                                    Name = subItem.Title,
                                    Slug = subItem.Slug,
                                    TopicId = subItem.TopicId
                                });
                            }
                            target.Lessons.Add(new LessonIndexViewModel
                            {
                                LessonId = item.LessonId,
                                Name = item.Name,
                                Slug = item.Slug,
                                Topics = newTopics
                            });
                        }
                        
                    }
                }

                return target;
            
        }

        public IEnumerable<CourseListViewModel> GetCoursesByCategorId(int id, bool isAuthenticated = false)
        {

            if (isAuthenticated)
            {
                var sourceCollection = _context.Course.Where(x => x.CategoryId == id).Select(x => new CourseListViewModel
                {
                    CourseId = x.CourseId,
                    Name = x.Name,
                    Slug = x.Slug,
                    CategoryId = x.CategoryId,
                    Category = _context.Category.FirstOrDefault(y => y.CategoryId == x.CategoryId).Name
                });
                return sourceCollection;
            }
            else
            {
                var sourceCollection = _context.Course.Where(x => x.CategoryId == id && x.IsActive).Select(x => new CourseListViewModel
                {
                    CourseId = x.CourseId,
                    Name = x.Name,
                    Slug = x.Slug,
                    CategoryId = x.CategoryId,
                    Category = _context.Category.FirstOrDefault(y => y.CategoryId == x.CategoryId && x.IsActive).Name
                });
                return sourceCollection;
            }
        }
    }
}

