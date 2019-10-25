namespace LearningApiCore.DataAccess.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Course : EntityBase
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Slug { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual List<Lesson> Lessons { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
