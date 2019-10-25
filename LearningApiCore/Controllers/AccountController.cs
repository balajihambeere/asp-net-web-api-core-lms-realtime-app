namespace LearningApiCore.Controllers
{
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Infrastructure;
    using LearningApiCore.Infrastructure.Errors;
    using LearningApiCore.Infrastructure.Security;
    using LearningApiCore.ViewModels;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtTokenCreator _jwtTokenCreator;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtTokenCreator jwtTokenCreator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenCreator = jwtTokenCreator;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel source)
        {
            if (string.IsNullOrWhiteSpace(source.UserName))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "required input parameter");
            }

            //object initalizer and map username and phonenumber
            var appUser = new AppUser
            {
                UserName = source.UserName.Trim(),
                PhoneNumber = source.UserName.Trim()
            };

            //register the user
            var result = await _userManager.CreateAsync(appUser, source.Password);

            //check creation success 
            if (result.Succeeded)
            {
                //generate token
                var code = await _userManager.GenerateChangePhoneNumberTokenAsync(appUser, appUser.UserName);
                //send token to registered phone number

                //redirect to verify phonenumber route
                return Ok(new { userName = source.UserName });
            }
            return GetErrorResult(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "required input parameter");
            }
            //get application user by user name
            var appUser = await _userManager.FindByNameAsync(model.UserName.Trim());
            if (appUser == null)
            {
                throw new ApiException(HttpStatusCode.Unauthorized, "User does not exists");
            }

            AuthViewModel user = new AuthViewModel { };

            //sign in to application
            var result = await _signInManager.PasswordSignInAsync(appUser.UserName, model.Password, false, false);
            if (result.Succeeded)
            {
                user.UserName = appUser.UserName;
                user.Token = await _jwtTokenCreator.CreateToken(appUser.UserName, appUser);
                user.RedirectUrl = RouteConstants.DashBoard;
                return Ok(user);
            }

            throw new ApiException(HttpStatusCode.BadRequest, "password is incorrect");
        }

        public IActionResult GetErrorResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                if (result.Errors != null)
                {

                    foreach (IdentityError error in result.Errors)
                    {
                        errors.Add(error.Description);
                        ModelState.AddModelError("", error.Description);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }
                // string content = JsonConvert.SerializeObject(new { errors });
                return BadRequest(new { errors });
            }
            return null;
        }
    }
}