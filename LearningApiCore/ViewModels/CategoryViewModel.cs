namespace LearningApiCore.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }



    public class SelectCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}
