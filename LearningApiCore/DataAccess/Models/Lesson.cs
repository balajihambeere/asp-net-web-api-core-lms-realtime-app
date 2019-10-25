namespace LearningApiCore.DataAccess.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Lesson : EntityBase
    {
        public int LessonId { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Slug { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public virtual List<Topic> Topics { get; set; }
    }
}
