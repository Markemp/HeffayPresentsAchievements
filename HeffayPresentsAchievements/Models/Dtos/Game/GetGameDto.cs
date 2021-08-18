using System;

namespace HeffayPresentsAchievements.Models.Dtos.Game
{
    public record GetGameDto
    (
        Guid Id,
        string Name
    );
}
