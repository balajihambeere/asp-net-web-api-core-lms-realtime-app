namespace LearningApiCore.Controllers
{
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Interfaces;
    using LearningApiCore.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Net;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/lesson")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonsController(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        [HttpGet]
        public IActionResult GetLesson()
        {
            var results = new ObjectResult(_lessonRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        [HttpGet("{id}", Name = "GetLesson")]
        
        public async Task<IActionResult> GetLesson([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var lesson = await _lessonRepository.Find(id);

            if (lesson == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                lesson.LessonId,
                lesson.Name,
                lesson.IsActive,
                lesson.SortOrder,
                lesson.CourseId,
                lesson.Course.CategoryId
            });
        }

        [HttpPost]
        public async Task<IActionResult> PostLesson([FromBody] PostLessonViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //if (await _lessonRepository.Exists(source.Name))
            //{
            //    return BadRequest(new { errors = "Already Exists" });
            //}
            var result = await _lessonRepository.Add(new Lesson
            {
                Slug = Slug.GenerateSlug(source.Name),
                CourseId = source.CourseId,
                Name = source.Name,
                IsActive = source.IsActive,
                SortOrder = source.SortOrder
            });
            return CreatedAtAction("getLesson", new { id = result.LessonId }, source);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLesson([FromRoute] int id, [FromBody] PostLessonViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != source.LessonId)
            {
                return BadRequest();
            }

            var lesson = await _lessonRepository.Find(id);
            if (lesson == null)
            {
                return NotFound();
            }

            lesson.Slug = Slug.GenerateSlug(source.Name);
            lesson.CourseId = source.CourseId;
            lesson.SortOrder = source.SortOrder;
            lesson.Name = source.Name;
            lesson.IsActive = source.IsActive;

            try
            {
                await _lessonRepository.Update(lesson);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _lessonRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpGet("getLessonByCourse/{id}")]
        public IActionResult GetLessonByCourse([FromRoute] int id)
        {
            var results = new ObjectResult(_lessonRepository.GetLessonByCourse(id))
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        [HttpGet]
        [Route("getKeyValueByCourse/{id}")]
        public IActionResult GetKeyValueByCourse([FromRoute] int id)
        {
            var results = new ObjectResult(_lessonRepository.GetKeyValueByCourse(id))
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }
    }
}