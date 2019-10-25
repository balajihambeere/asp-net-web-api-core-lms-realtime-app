namespace LearningApiCore.Interfaces
{
    using LearningApiCore.ViewModels;
    using System.Collections.Generic;

    public interface IHomeRepository
    {
        CourseIndexViewModel GetCourseIndex(string slug, bool isAuthenticated = false);
        CourseIndexViewModel GetCourseIndex(int id, bool isAuthenticated = false);
        IEnumerable<CourseListViewModel> GetCoursesByCategorName(string name, bool isAuthenticated = false);
        IEnumerable<CourseListViewModel> GetCoursesByCategorId(int id, bool isAuthenticated = false);

    }
}
