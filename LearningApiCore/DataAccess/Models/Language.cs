namespace LearningApiCore.DataAccess.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Language : EntityBase
    {
        public int LanguageId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
