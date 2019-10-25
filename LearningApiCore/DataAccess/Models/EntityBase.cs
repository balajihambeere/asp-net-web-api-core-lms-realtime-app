namespace LearningApiCore.DataAccess.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public abstract class EntityBase
    {
        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Required]
        public Guid RowId { get; set; }
    }
}
