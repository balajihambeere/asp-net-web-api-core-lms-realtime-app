namespace LearningApiCore.Controllers
{
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Infrastructure.Errors;
    using LearningApiCore.Interfaces;
    using LearningApiCore.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Net;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/category")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository, ICurrentUserAccessor currentUserAccessor)
        {
            _categoryRepository = categoryRepository;
            _currentUserAccessor = currentUserAccessor;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCategory()
        {
            var results = new ObjectResult(_categoryRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        [HttpGet("{id}", Name = "GetCategory")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var source = await _categoryRepository.Find(id);
            return Ok(new { source.CategoryId, source.Name, source.IsActive });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostCategory([FromBody] CategoryViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _categoryRepository.Exists(source.Name))
            {
                return BadRequest(new { errors = "Already Exists" });
            }
            var result = await _categoryRepository.Add(new Category { Name = source.Name, IsActive = source.IsActive, Slug = Slug.GenerateSlug(source.Name) });
            return CreatedAtAction("getCategory", new { id = result.CategoryId }, source);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutCategory([FromRoute] int id, [FromBody] CategoryViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != source.CategoryId)
            {
                return BadRequest();
            }
            try
            {
                var category = await _categoryRepository.Find(id);
                category.Name = source.Name;
                category.IsActive = source.IsActive;
                category.Slug = Slug.GenerateSlug(source.Name);

                await _categoryRepository.Update(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _categoryRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "some thing went wrong with update! Please contact with your administrator");
                }
            }
            return NoContent();
        }

        [HttpGet]
        [Route("getKeyValue")]
        public IActionResult GetKeyValue()
        {
            //authenticated users
            if (!string.IsNullOrWhiteSpace(_currentUserAccessor.GetCurrentUsername()))
            {
                //for authenticated user get all inactive and active categories
                var results = new ObjectResult(_categoryRepository.GetkeyValue(isAuthenticated: true))
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
            else
            {
                //for non authenticated user get all  active categories
                var results = new ObjectResult(_categoryRepository.GetkeyValue())
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return results;
            }
        }
    }
}