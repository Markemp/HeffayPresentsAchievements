using System.ComponentModel.DataAnnotations;

namespace HeffayPresentsAchievements.Models.Dtos.User
{
    public record UserRegisterDto(
        [Display(Name = "User name")]
        [Required(ErrorMessage = "A user name is required")]
        string Username,
        [Required(ErrorMessage = "A password is required")]
        string Password,
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        string Email
    );
}
