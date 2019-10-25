namespace LearningApiCore.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TopicViewModel
    {
        public TopicViewModel()
        {
            Lesson = new SelectLessonViewModel();
        }
        public int TopicId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string HtmlContent { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public SelectLessonViewModel Lesson { get; set; }
    }

    public class PostTopicViewModel
    {
        public PostTopicViewModel()
        {
            Contents = new List<StoreContentViewModel>();
        }
        public int TopicId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int LessonId { get; set; }

        public int CategoryId { get; set; }

        public int CourseId { get; set; }

        public ICollection<StoreContentViewModel> Contents { get; set; }
    }

    public class EditorViewModel
    {
        public EditorViewModel()
        {
            HtmlContent = new ContentViewModel();
            Code = new CodeViewModel();
        }
        public int Key { get; set; }
        public ContentViewModel HtmlContent { get; set; }
        public CodeViewModel Code { get; set; }
    }

    public class TopicIndexViewModel
    {
        public int TopicId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    public class StoreContentViewModel
    {
        public StoreContentViewModel()
        {
            HtmlContent = new ContentViewModel();
            Code = new CodeViewModel();
            Image = new ImageViewModel();
        }
        public int Key { get; set; }
        public ContentViewModel HtmlContent { get; set; }
        public CodeViewModel Code { get; set; }
        public ImageViewModel Image { get; set; }
    }

    public class CodeViewModel
    {
        public int Key { get; set; }
        public string Language { get; set; }
        public string Code { get; set; }
    }

    public class ContentViewModel
    {
        public int Key { get; set; }
        public string HtmlContent { get; set; }
    }

    public class ImageViewModel
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
