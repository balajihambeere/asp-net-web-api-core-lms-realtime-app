namespace LearningApiCore.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class CourseViewModel
    {
        public CourseViewModel()
        {
            KeyValue = new KeyValueViewModel();
        }
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public KeyValueViewModel KeyValue { get; set; }
    }

    public class PostCourseViewModel
    {
        public int CourseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int CategoryId { get; set; }
    }

    public class SelectCourseViewModel
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
    }

    public class CourseListViewModel
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string CategorySlug { get; set; }
    }

    public class CourseIndexViewModel
    {
        public CourseIndexViewModel()
        {
            Lessons = new List<LessonIndexViewModel>();
        }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Category { get; set; }
        public string CategorySlug { get; set; }

        public ICollection<LessonIndexViewModel> Lessons { get; set; }
    }
}
