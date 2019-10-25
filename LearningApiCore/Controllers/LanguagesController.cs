namespace LearningApiCore.Controllers
{
    using LearningApiCore.DataAccess.Models;
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
    [Route("api/language")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguagesController(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        [HttpGet]
        public IActionResult GetLanguage()
        {
            var results = new ObjectResult(_languageRepository.GetAll())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLanguage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            LanguageViewModel target = new LanguageViewModel { };

            var source = await _languageRepository.Find(id);
            return Ok(new { source.LanguageId, source.Name, source.IsActive });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostLanguage([FromBody] LanguageViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _languageRepository.Exists(source.Name))
            {
                return BadRequest(new { errors = "Already Exists" });
            }
            var result = await _languageRepository.Add(new Language { Name = source.Name, IsActive = source.IsActive });
            return CreatedAtAction("getLanguage", new { id = result.LanguageId }, source);
        }

        [HttpPut("{id}", Name = "GetLanguage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutLanguage([FromRoute] int id, [FromBody] LanguageViewModel source)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != source.LanguageId)
            {
                return BadRequest();
            }

            try
            {
                var language = await _languageRepository.Find(id);
                language.Name = source.Name;
                language.IsActive = source.IsActive;

                await _languageRepository.Update(language);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _languageRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "some thing went wrong with update!");
                }
            }
            return NoContent();
        }

        [HttpGet]
        [Route("getKeyValue")]
        public IActionResult GetKeyValue()
        {
            var results = new ObjectResult(_languageRepository.GetkeyValue())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return results;
        }
    }
}
