namespace LearningApiCore.Controllers
{
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Infrastructure.Helpers;
    using LearningApiCore.Interfaces;
    using LearningApiCore.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/topic")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;
        private readonly ICourseRepository _courseRepository;

        public TopicsController(ITopicRepository topicRepository, ICourseRepository courseRepository)
        {
            _topicRepository = topicRepository;
            _courseRepository = courseRepository;
        }

        [HttpGet]
        public IActionResult GetTopic()
        {
            var results = new ObjectResult(_topicRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var topic = await _topicRepository.Find(id);
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostTopic()
        {
            var source = JsonConvert.DeserializeObject<PostTopicViewModel>(Request.Form["source"]);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _topicRepository.Exists(source.Title, source.LessonId))
            {
                return BadRequest(new { errors = "Already Exists" });
            }
            Topic target = new Topic
            {
                Slug = Slug.GenerateSlug(source.Title),
                LessonId = source.LessonId,
                Title = source.Title,
                IsActive = source.IsActive,
                CreatedOn = DateTime.Now,
                RowId = Guid.NewGuid()
            };

            var result = await _topicRepository.Add(target);
            await StoreToAzureTable(source, target);
            return CreatedAtAction("getCategory", new { id = result.TopicId }, source);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutTopic([FromRoute] int id)
        {
            var source = JsonConvert.DeserializeObject<PostTopicViewModel>(Request.Form["source"]);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != source.TopicId)
            {
                return BadRequest();
            }

            var target = await _topicRepository.Find(id);
            target.Slug = Slug.GenerateSlug(source.Title);
            target.LessonId = source.LessonId;
            target.IsActive = source.IsActive;
            target.Title = source.Title;

            if (target == null)
            {
                return NotFound();
            }

            try
            {
                await _topicRepository.Update(target);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _topicRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (target != null)
            {
                await StoreToAzureTable(source, target);
            }

            return NoContent();
        }

        [HttpGet("getTopicByLesson/{id}")]
        public IActionResult getTopicByLesson([FromRoute] int id)
        {
            var results = new ObjectResult(_topicRepository.GetTopicByLesson(id))
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        #region Private Methods

        private async Task StoreToAzureTable(PostTopicViewModel source, Topic target)
        {
            List<StoreContentViewModel> contentList = new List<StoreContentViewModel>();

            foreach (var item in source.Contents)
            {
                ImageViewModel model = new ImageViewModel();
                if (item.Image != null)
                {
                    if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                    {
                        foreach (IFormFile formFile in Request.Form.Files)
                        {
                            if (formFile.Name.Equals(item.Image.Name))
                            {
                                model.Key = item.Image.Key;
                                model.Name = item.Image.Name;
                                model.ImageUrl = await SavePhoto(formFile);
                            }
                        }
                    }
                }
                contentList.Add(new StoreContentViewModel
                {
                    HtmlContent = item.HtmlContent,
                    Code = item.Code,
                    Image = model
                });
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            //convert to content to json
            var content = JsonConvert.SerializeObject(contentList, Formatting.None, settings);
            //Store html content data  on the table storage
            await StorageHelper.InsertOrMerge(target.RowId.ToString(), target.TopicId.ToString(), content);
        }

        private async Task<string> SavePhoto(IFormFile file)
        {
            return await StorageHelper.BlobStorage(file);
        }
        #endregion
    }
}