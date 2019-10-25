namespace LearningApiCore.DataAccess.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Topic : EntityBase
    {
        public int TopicId { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Slug { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
