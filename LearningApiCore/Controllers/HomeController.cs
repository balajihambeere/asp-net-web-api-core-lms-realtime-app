namespace LearningApiCore.Controllers
{
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Infrastructure.Helpers;
    using LearningApiCore.Interfaces;
    using LearningApiCore.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly ITopicRepository _topicRepository;
        public HomeController(ICurrentUserAccessor currentUserAccessor, ICategoryRepository categoryRepository,
            ICourseRepository courseRepository, ILessonRepository lessonRepository, IHomeRepository homeRepository, ITopicRepository topicRepository)
        {
            _currentUserAccessor = currentUserAccessor;
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _homeRepository = homeRepository;
            _topicRepository = topicRepository;
        }

        [HttpGet]
        [Route("getCategories")]
        public IActionResult GetCategories()
        {
            //authenticated users
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                //for authenticated user get all inactive and active courses
                var results = new ObjectResult(_categoryRepository.GetkeyValue(isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                //for non authenticated user get active categories
                var results = new ObjectResult(_categoryRepository.GetkeyValue())
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }

        [HttpGet]
        [Route("getCourses")]
        public IActionResult GetCourses()
        {
            List<CourseListViewModel> collection = new List<CourseListViewModel>();
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                //for authenticated user get all inactive and active courses
                foreach (var item in _courseRepository.GetAll(isAuthenticated: true))
                {
                    var category = _categoryRepository.Find(item.CategoryId).Result;
                    collection.Add(new CourseListViewModel
                    {
                        CourseId = item.CourseId,
                        CategoryId = item.CategoryId,
                        Name = item.Name,
                        Slug = item.Slug,
                        Category = category.Name,
                        CategorySlug = category.Slug
                    });
                }
                var results = new ObjectResult(collection)
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                //for non authenticated user get active courses
                foreach (var item in _courseRepository.GetAll())
                {
                    var category = _categoryRepository.Find(item.CategoryId).Result;
                    collection.Add(new CourseListViewModel
                    {
                        CourseId = item.CourseId,
                        CategoryId = item.CategoryId,
                        Name = item.Name,
                        Slug = item.Slug,
                        Category = category.Name,
                        CategorySlug = category.Slug
                    });
                }
                var results = new ObjectResult(collection)
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }

        [HttpGet]
        [Route("getCoursesByCategorName/{name}")]
        public IActionResult getCoursesByCategorName([FromRoute] string name)
        {
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                //for authenticated user get all inactive and active courses by category
                var results = new ObjectResult(_homeRepository.GetCoursesByCategorName(name, isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                //for non authenticated user get active courses by category
                var results = new ObjectResult(_homeRepository.GetCoursesByCategorName(name))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }

        [HttpGet]
        [Route("getCoursesBySlug/{slug}")]
        public IActionResult GetCourses([FromRoute] string slug)
        {
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                var results = new ObjectResult(_homeRepository.GetCourseIndex(slug, isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                var results = new ObjectResult(_homeRepository.GetCourseIndex(slug))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }


        [HttpGet]
        [Route("getCourseByName/{name}")]
        public IActionResult GetCourseByName([FromRoute] string name)
        {
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                var source = _courseRepository.FindBySlug(name).Result;
                if (source == null)
                {
                    return BadRequest();
                }
                var category = _categoryRepository.Find(source.CategoryId).Result;
                if (category == null)
                {
                    return BadRequest();
                }
                var result = StorageHelper.GetTableEntity(source.RowId.ToString(), source.CourseId.ToString()).Result;
                var results = new ObjectResult(new { source.CategoryId, source.Name, source.IsActive, description = result.Content, source.CourseId })
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                var source = _courseRepository.FindBySlug(name).Result;
                var result = StorageHelper.GetTableEntity(source.RowId.ToString(), source.CourseId.ToString()).Result;
                var results = new ObjectResult(new { source.CategoryId, source.Name, source.IsActive, description = result.Content, source.CourseId })
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }


        [HttpGet]
        [Route("getTopicById/{topicId}")]
        public async Task<IActionResult> getTopicById([FromRoute] int topicId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var topic = await _topicRepository.Find(topicId);
            if (topic == null)
            {
                return NotFound();
            }

            var blogEntity = await StorageHelper.GetTableEntity(topic.RowId.ToString(), topic.TopicId.ToString());
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };


            var topicVM = new PostTopicViewModel
            {
                TopicId = topic.TopicId,
                Title = topic.Title,
                Contents = JsonConvert.DeserializeObject<List<StoreContentViewModel>>(blogEntity.Content, settings),
                IsActive = topic.IsActive,
                LessonId = topic.LessonId,
                CourseId = topic.Lesson.CourseId,
                CategoryId = _courseRepository.Find(topic.Lesson.CourseId).Result.CategoryId
            };
            return Ok(topicVM);
        }

        [HttpGet]
        [Route("getCoursesByCategorId/{id}")]
        public IActionResult GetCoursesByCategorId([FromRoute] int id)
        {
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                //for authenticated user get all inactive and active courses by category
                var results = new ObjectResult(_homeRepository.GetCoursesByCategorId(id, isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                //for non authenticated user get active courses by category
                var results = new ObjectResult(_homeRepository.GetCoursesByCategorId(id))

                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }


        [HttpGet]
        [Route("getCourses/{id}")]
        public IActionResult GetCourses([FromRoute] int id)
        {
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                var results = new ObjectResult(_homeRepository.GetCourseIndex(id, isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                var results = new ObjectResult(_homeRepository.GetCourseIndex(id))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }
    }
}
