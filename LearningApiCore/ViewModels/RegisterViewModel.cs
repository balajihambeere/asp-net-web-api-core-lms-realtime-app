using LearningApiCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace LearningApiCore.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [PhoneNumber(ErrorMessage = "entered valid phone number")]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
