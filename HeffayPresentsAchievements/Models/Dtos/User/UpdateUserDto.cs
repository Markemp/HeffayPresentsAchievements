using System;

namespace HeffayPresentsAchievements.Models.Dtos.User
{
    public record UpdateUserDto(
        Guid Id,
        string Name,
        string Email
    );
}
