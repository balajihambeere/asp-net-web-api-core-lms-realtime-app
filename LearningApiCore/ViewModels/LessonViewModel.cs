using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LearningApiCore.ViewModels
{
    public class LessonViewModel
    {
        public LessonViewModel()
        {
            Course = new SelectCourseViewModel();
        }
        public int LessonId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public SelectCourseViewModel Course { get; set; }
    }


    public class PostLessonViewModel
    {
        public int LessonId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int CourseId { get; set; }

        public int CategoryId { get; set; }
    }
    public class SelectLessonViewModel
    {
        public int LessonId { get; set; }
        public string Name { get; set; }
    }

    public class LessonIndexViewModel
    {
        public LessonIndexViewModel()
        {
            Topics = new List<TopicIndexViewModel>();
        }
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public ICollection<TopicIndexViewModel> Topics { get; set; }
    }
}
