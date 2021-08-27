using System;

namespace HeffayPresentsAchievements.Models.Dtos.User
{
    public record GetUserDto(
        string? Name,
        Guid Id
    );
}
