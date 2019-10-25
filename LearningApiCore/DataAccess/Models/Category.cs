namespace LearningApiCore.DataAccess.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Category : EntityBase
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Slug { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public List<Course> Courses { get; set; }
    }
}
