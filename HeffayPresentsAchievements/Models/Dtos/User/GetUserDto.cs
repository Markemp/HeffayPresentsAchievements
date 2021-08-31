using System;

namespace HeffayPresentsAchievements.Models.Dtos.User
{
    public record GetUserDto(
        Guid Id,
        string Name,
        string Email
    );
}
