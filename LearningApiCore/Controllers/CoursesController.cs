namespace LearningApiCore.Controllers
{
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Infrastructure.Errors;
    using LearningApiCore.Infrastructure.Helpers;
    using LearningApiCore.Interfaces;
    using LearningApiCore.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Net;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/course")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        public CoursesController(ICourseRepository courseRepository, ICurrentUserAccessor currentUserAccessor)
        {
            _courseRepository = courseRepository;
            _currentUserAccessor = currentUserAccessor;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCourse()
        {
            var results = new ObjectResult(_courseRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        [HttpGet("{id}", Name = "GetCourse")]
        public async Task<IActionResult> GetCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var source = await _courseRepository.Find(id);
            var result = await StorageHelper.GetTableEntity(source.RowId.ToString(), source.CourseId.ToString());
            return Ok(new { source.CategoryId, source.Name, source.IsActive, description = result.Content, source.CourseId });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostCourse([FromBody] PostCourseViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _courseRepository.Exists(source.Name))
            {
                return BadRequest(new { errors = "Already Exists" });
            }
            var target = await _courseRepository.Add(new Course
            {
                CategoryId = source.CategoryId,
                Slug = Slug.GenerateSlug(source.Name),
                Name = source.Name,
                IsActive = source.IsActive,
            });
            await StorageHelper.InsertOrMerge(target.RowId.ToString(), target.CourseId.ToString(), source.Description);

            return CreatedAtAction("getCourse", new { id = target.CourseId }, source);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutCourse([FromRoute] int id, [FromBody] PostCourseViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != source.CourseId)
            {
                return BadRequest();
            }

            var course = await _courseRepository.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            try
            {
                course.Slug = Slug.GenerateSlug(source.Name);
                course.CategoryId = source.CategoryId;
                course.Name = source.Name;
                course.IsActive = source.IsActive;
                await _courseRepository.Update(course);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _courseRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "some thing went wrong with update!");
                }
            }

            await StorageHelper.InsertOrMerge(course.RowId.ToString(), course.CourseId.ToString(), source.Description);
            return NoContent();
        }

        [HttpGet("getCourseByCategory/{id}")]
        public IActionResult getCourseByCategory([FromRoute] int id)
        {
            //authenticated users
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                //for authenticated user get all inactive and active courses
                var results = new ObjectResult(_courseRepository.GetCourseByCategory(id, isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                var results = new ObjectResult(_courseRepository.GetCourseByCategory(id))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }

        [HttpGet]
        [Route("getKeyValueByCategory/{id}")]
        public IActionResult GetKeyValueByCategory([FromRoute] int id)
        {
            var results = new ObjectResult(_courseRepository.GetKeyValueByCategory(id))
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }
    }
}