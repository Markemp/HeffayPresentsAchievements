using System.ComponentModel.DataAnnotations;

namespace HeffayPresentsAchievements.Models.Dtos.User
{
    public record UserLoginDto(
        [Display(Name = "User name")]
        [Required(ErrorMessage = "A user name is required")]
        string Username,
        [Required(ErrorMessage = "A password is required")]
        string Password
    );
}
